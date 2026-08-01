using Domiki.Web.Village.Models;

namespace Domiki.Web.Village;

/// <summary>
/// Справочник долин: имя и вид места, где может встать новая деревня.
/// </summary>
/// <remarks>
/// Модификатора местности у долины нет – в первой версии это только косметика (GAMEDESIGN.md §3 Слой 4), поэтому
/// справочник живёт в коде рядом с видом, а не в БД.
/// </remarks>
public static class RelocationValleys
{
    /// <summary>
    /// Долина, с которой начинается игра.
    /// </summary>
    public const int StartingValleyId = 0;

    /// <summary>
    /// Долины, из которых игрок выбирает новое место при переезде.
    /// </summary>
    public static readonly Valley[] Choices =
    [
        new()
        {
            Id = 1,
            Name = "Липовый лог",
            LogicName = "linden_hollow",
            Description = "Пологие луга, по склонам липняк, ни дыма, ни изгороди.",
        },
        new()
        {
            Id = 2,
            Name = "Светлый плёс",
            LogicName = "bright_reach",
            Description = "Река разливается вширь, по берегам ивняк и песок.",
        },
        new()
        {
            Id = 3,
            Name = "Белый яр",
            LogicName = "white_bluff",
            Description = "Меловые обрывы, поверху – сухие травы и высокое небо.",
        },
    ];

    /// <summary>
    /// Возвращает долину по идентификатору.
    /// </summary>
    /// <param name="valleyId">Идентификатор долины.</param>
    /// <returns>Долина справочника либо стартовая долина, если идентификатор ей не принадлежит.</returns>
    public static Valley Get(int valleyId)
    {
        return Choices.FirstOrDefault(x => x.Id == valleyId) ?? Starting;
    }

    /// <summary>
    /// Есть ли такая долина в выборе при переезде.
    /// </summary>
    /// <param name="valleyId">Идентификатор долины.</param>
    /// <returns><see langword="true"/>, если в эту долину можно переехать.</returns>
    public static bool IsChoice(int valleyId)
    {
        return Choices.Any(x => x.Id == valleyId);
    }

    private static Valley Starting { get; } = new()
    {
        Id = StartingValleyId,
        Name = "Родная сторона",
        LogicName = "home_side",
        Description = "Земля, с которой всё начиналось.",
    };
}
