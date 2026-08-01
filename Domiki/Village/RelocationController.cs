using Domiki.Web.Core;
using Domiki.Web.Data.Entities;
using Domiki.Web.Infrastructure;
using Domiki.Web.Village.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Domiki.Web.Village;

/// <summary>
/// Переезд в новую долину и лесенка перков узелков памяти.
/// </summary>
public class RelocationController : GameControllerBase
{
    private readonly RelocationManager _relocationManager;
    private readonly PerkManager _perkManager;

    public RelocationController(DomikManager domikManager, RelocationManager relocationManager, PerkManager perkManager)
        : base(domikManager)
    {
        _relocationManager = relocationManager;
        _perkManager = perkManager;
    }

    /// <summary>
    /// Сборы обоза: что уедет, что останется, сколько узелков даст деревня и куда можно уехать.
    /// </summary>
    /// <returns>Сборы для диалога подтверждения.</returns>
    /// <remarks>
    /// Гейт, узелки в остатке и лесенка перков едут в снимке состояния игры (<see cref="Infrastructure.Dto.GameStateDto.Relocation"/>).
    /// </remarks>
    [HttpGet]
    [Route("/Domiki/GetRelocation")]
    public RelocationPlanDto GetRelocation()
    {
        return _relocationManager.GetPlan(GetPlayerId()).ToDto();
    }

    /// <summary>
    /// Памятный столб – личная страница прожитых деревень.
    /// </summary>
    /// <returns>Столб игрока.</returns>
    [HttpGet]
    [Route("/Domiki/GetMemorialPost")]
    public MemorialPostDto GetMemorialPost()
    {
        return _relocationManager.GetMemorialPost(GetPlayerId()).ToDto();
    }

    /// <summary>
    /// Переезд в новую долину: деревня остаётся на памятном столбе, артель и чертежи едут дальше.
    /// </summary>
    /// <param name="request">Выбранная долина и, если игрок переименовался, новое имя деревни.</param>
    [HttpPost]
    [Route("/Domiki/Relocate")]
    public void Relocate([FromBody] RelocateDto request)
    {
        _relocationManager.Relocate(GetPlayerId(), request?.ValleyId ?? 0, request?.VillageName);
    }

    /// <summary>
    /// Покупает следующую ступень перка за узелки памяти.
    /// </summary>
    /// <param name="perkType">Перк лесенки – значение <see cref="RelocationPerkType"/>.</param>
    [HttpPost]
    [Route("/Domiki/BuyPerk")]
    public void BuyPerk([FromQuery] int perkType)
    {
        _perkManager.BuyPerk(GetPlayerId(), (RelocationPerkType)perkType);
    }
}
