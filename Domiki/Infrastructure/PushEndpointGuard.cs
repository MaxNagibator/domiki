using System.Net;
using System.Net.Sockets;

namespace Domiki.Web.Infrastructure;

/// <summary>
/// Проверка адреса web-push подписки: допускает только внешние https-эндпоинты сервисов доставки,
/// отсекая приватные, петлевые и служебные адреса, чтобы браузерный <c>endpoint</c> не превратился
/// в серверный запрос к внутренней сети (SSRF).
/// </summary>
/// <remarks>
/// Две линии обороны. <see cref="EnsureRegisterable"/> на приёме подписки делает только дешёвую
/// проверку без обращения к сети: схема, отсутствие явно внутреннего хоста-литерала. <see cref="IsSendable"/>
/// перед самой отправкой резолвит доменное имя и отклоняет адрес, если хоть один из полученных IP –
/// приватный или служебный; именно здесь закрывается подмена (DNS rebinding) между моментом подписки и отправкой.
/// </remarks>
public static class PushEndpointGuard
{
    /// <summary>
    /// Проверяет адрес при регистрации подписки и бросает <see cref="BusinessException"/>, если он недопустим.
    /// </summary>
    /// <param name="endpoint">Адрес push-эндпоинта, присланный браузером.</param>
    public static void EnsureRegisterable(string endpoint)
    {
        if (!TryGetHttpsHost(endpoint, out var host) || IsForbiddenHostLiteral(host))
        {
            throw new BusinessException("Недопустимый адрес push-подписки");
        }
    }

    /// <summary>
    /// Проверяет адрес непосредственно перед отправкой уведомления, резолвя доменное имя в IP.
    /// </summary>
    /// <param name="endpoint">Адрес push-эндпоинта из сохранённой подписки.</param>
    /// <returns><see langword="true"/>, если адрес внешний и безопасен для серверного запроса; иначе <see langword="false"/>.</returns>
    public static bool IsSendable(string endpoint)
    {
        if (!TryGetHttpsHost(endpoint, out var host) || host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (IPAddress.TryParse(host, out var literal))
        {
            return !IsPrivateOrReserved(literal);
        }

        IPAddress[] resolved;
        try
        {
            // TODO: остаётся гонка resolve-then-connect (WebPushClient резолвит хост заново при подключении);
            // при ужесточении – слать через HttpClient с фиксацией проверенного IP или egress-allowlist
            resolved = Dns.GetHostAddresses(host);
        }
        catch (SocketException)
        {
            return false;
        }

        return resolved.Length > 0 && resolved.All(ip => !IsPrivateOrReserved(ip));
    }

    private static bool TryGetHttpsHost(string endpoint, out string host)
    {
        host = string.Empty;
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
        {
            return false;
        }

        host = uri.DnsSafeHost;
        return !string.IsNullOrEmpty(host);
    }

    private static bool IsForbiddenHostLiteral(string host)
    {
        return host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
               || (IPAddress.TryParse(host, out var ip) && IsPrivateOrReserved(ip));
    }

    private static bool IsPrivateOrReserved(IPAddress address)
    {
        var ip = address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address;

        if (IPAddress.IsLoopback(ip) || ip.Equals(IPAddress.Any) || ip.Equals(IPAddress.IPv6Any))
        {
            return true;
        }

        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            var octets = ip.GetAddressBytes();
            return octets[0] == 0
                   || octets[0] == 10
                   || (octets[0] == 172 && octets[1] >= 16 && octets[1] <= 31)
                   || (octets[0] == 192 && octets[1] == 168)
                   || (octets[0] == 169 && octets[1] == 254)
                   || (octets[0] == 100 && octets[1] >= 64 && octets[1] <= 127)
                   || octets[0] >= 224;
        }

        if (ip.AddressFamily == AddressFamily.InterNetworkV6)
        {
            return ip.IsIPv6LinkLocal
                   || ip.IsIPv6SiteLocal
                   || ip.IsIPv6Multicast
                   || (ip.GetAddressBytes()[0] & 0xFE) == 0xFC;
        }

        return true;
    }
}
