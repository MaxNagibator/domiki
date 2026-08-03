namespace Domiki.Web.Village.Dto;

/// <summary>
/// Состояние переезда в новую долину: гейт, узелки памяти и лесенка перков.
/// </summary>
/// <remarks>
/// Едет в снимке состояния игры – порог нужен на любом экране, поэтому здесь только то, что отвечается дешёвыми
/// запросами. Сборы обоза (<see cref="RelocationController.GetRelocation"/>) и памятный столб
/// (<see cref="RelocationController.GetMemorialPost"/>) берутся отдельными запросами при открытии раздела.
/// </remarks>
public sealed record RelocationDto
{
    /// <summary>
    /// Обжитость деревни, с которой откроется ближайший переезд.
    /// </summary>
    /// <remarks>
    /// Растёт с каждым переездом (см. <see cref="Village.VillageLevelCalculator.GetRelocationThreshold"/>); показывается
    /// игроку не числом, а сроком <see cref="EstimatedDays"/> – число уходит во всплывающую подсказку.
    /// </remarks>
    public required int Threshold { get; init; }

    /// <summary>
    /// Нынешняя обжитость деревни.
    /// </summary>
    public required int Level { get; init; }

    /// <summary>
    /// Оценка срока до порога по нынешнему ходу деревни в сутках.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> – ход не с чего считать или порог уже взят.
    /// </remarks>
    public int? EstimatedDays { get; init; }

    /// <summary>
    /// Момент, раньше которого переехать нельзя из-за кулдауна.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> – кулдауна нет.
    /// </remarks>
    public DateTime? CooldownUntil { get; init; }

    /// <summary>
    /// Готов ли переезд прямо сейчас.
    /// </summary>
    public required bool CanRelocate { get; init; }

    /// <summary>
    /// Что мешает переехать прямо сейчас.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> – ничего не мешает (<see cref="CanRelocate"/>).
    /// </remarks>
    public string? BlockReason { get; init; }

    /// <summary>
    /// Остаток нетраченых узелков памяти.
    /// </summary>
    public required int Knots { get; init; }

    /// <summary>
    /// Число уже совершённых переездов.
    /// </summary>
    public required int RelocationCount { get; init; }

    /// <summary>
    /// Долина, в которой стоит нынешняя деревня – ссылка на <see cref="ValleyDto.Id"/>.
    /// </summary>
    /// <value><c>0</c> – стартовая долина.</value>
    public required int ValleyId { get; init; }

    /// <summary>
    /// Название долины, в которой стоит нынешняя деревня.
    /// </summary>
    /// <remarks>
    /// Приезжает готовым: справочник долин <see cref="RelocationPlanDto.Valleys"/> едет только в сборах обоза,
    /// а стартовой долины в нём нет вовсе.
    /// </remarks>
    public required string ValleyName { get; init; }

    /// <summary>
    /// Лесенка перков со справочными ценами и купленными ступенями.
    /// </summary>
    public required PerkDto[] Perks { get; init; }
}

/// <summary>
/// Сборы обоза: что уедет, что останется, сколько узелков даст деревня и куда можно уехать.
/// </summary>
/// <remarks>
/// Берётся отдельным запросом при открытии раздела «Память» – в снимок состояния игры обход склада, двора и декора
/// не входит (см. <see cref="RelocationDto"/>).
/// </remarks>
public sealed record RelocationPlanDto
{
    /// <summary>
    /// Узелки памяти, которые деревня оставит на столбе при переезде прямо сейчас.
    /// </summary>
    public required int KnotsOnRelocate { get; init; }

    /// <summary>
    /// Поимённая сводка обеих колонок для первого шага подтверждения.
    /// </summary>
    public required RelocationSummaryDto Summary { get; init; }

    /// <summary>
    /// Долины, из которых выбирают новое место.
    /// </summary>
    public required ValleyDto[] Valleys { get; init; }
}

/// <summary>
/// Поимённая сводка переезда – числа для обеих колонок диалога подтверждения.
/// </summary>
public sealed record RelocationSummaryDto
{
    /// <summary>
    /// Сколько трудяг уедет со всей выучкой.
    /// </summary>
    public required int Workers { get; init; }

    /// <summary>
    /// Сколько чертежей уедет.
    /// </summary>
    public required int Blueprints { get; init; }

    /// <summary>
    /// Сколько золота уедет.
    /// </summary>
    /// <remarks>
    /// Не больше <see cref="Village.RelocationManager.GoldCarryCap"/>, излишек сгорает.
    /// </remarks>
    public required int Gold { get; init; }

    /// <summary>
    /// Сколько золота у игрока всего.
    /// </summary>
    public required int GoldTotal { get; init; }

    /// <summary>
    /// Сколько монет останется в оставленной деревне.
    /// </summary>
    public required int Coins { get; init; }

    /// <summary>
    /// Сколько единиц прочих припасов останется на складе.
    /// </summary>
    public required int Resources { get; init; }

    /// <summary>
    /// Сколько построек останется вместе со всеми уровнями.
    /// </summary>
    public required int Buildings { get; init; }

    /// <summary>
    /// Сколько монет будет в казне новой деревни.
    /// </summary>
    public required int StartingCoins { get; init; }
}

/// <summary>
/// Ступенчатый перк лесенки узелков памяти вместе с купленным уровнем.
/// </summary>
public sealed record PerkDto
{
    /// <summary>
    /// Перк лесенки – значение <see cref="Data.Entities.RelocationPerkType"/>.
    /// </summary>
    public required int PerkType { get; init; }

    /// <summary>
    /// Название перка.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Игровое описание перка.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Цена каждой ступени в узелках памяти, от первой к последней.
    /// </summary>
    public required int[] Costs { get; init; }

    /// <summary>
    /// Сколько ступеней уже куплено.
    /// </summary>
    public required int Level { get; init; }
}

/// <summary>
/// Долина, в которой может встать новая деревня, – имя и вид без модификатора местности.
/// </summary>
public sealed record ValleyDto
{
    /// <summary>
    /// Идентификатор долины.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Название долины.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Техническое имя долины для выбора вида на клиенте.
    /// </summary>
    public required string LogicName { get; init; }

    /// <summary>
    /// Короткое описание вида долины.
    /// </summary>
    public required string Description { get; init; }
}

/// <summary>
/// Памятный столб игрока – личная страница прожитых деревень.
/// </summary>
public sealed record MemorialPostDto
{
    /// <summary>
    /// Прожитые деревни, свежие первыми.
    /// </summary>
    public required MemorialVillageDto[] Villages { get; init; }

    /// <summary>
    /// Сумма обжитости всех прожитых деревень на дни их отъездов.
    /// </summary>
    public required int LevelSum { get; init; }

    /// <summary>
    /// Число совершённых переездов.
    /// </summary>
    public required int RelocationCount { get; init; }

    /// <summary>
    /// Дата первого дня игрока.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> – деревня заведена до среза «Переезд», и первый день не записан.
    /// </remarks>
    public DateTime? FirstDayDate { get; init; }
}

/// <summary>
/// Одна прожитая деревня на памятном столбе.
/// </summary>
public sealed record MemorialVillageDto
{
    /// <summary>
    /// Имя прожитой деревни.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> – деревня так и осталась безымянной.
    /// </remarks>
    public string? VillageName { get; init; }

    /// <summary>
    /// Индекс пиктограммы герба.
    /// </summary>
    public required int CrestIcon { get; init; }

    /// <summary>
    /// Индекс цвета герба.
    /// </summary>
    public required int CrestColor { get; init; }

    /// <summary>
    /// Долина, в которой стояла деревня – ссылка на <see cref="ValleyDto.Id"/>.
    /// </summary>
    public required int ValleyId { get; init; }

    /// <summary>
    /// Название долины, в которой стояла деревня.
    /// </summary>
    public required string ValleyName { get; init; }

    /// <summary>
    /// Обжитость на день отъезда.
    /// </summary>
    public required int Level { get; init; }

    /// <summary>
    /// Узелки памяти, начисленные за деревню.
    /// </summary>
    public required int Knots { get; init; }

    /// <summary>
    /// Сколько суток деревня прожила.
    /// </summary>
    public required int LivedDays { get; init; }

    /// <summary>
    /// Дата отъезда.
    /// </summary>
    public required DateTime Date { get; init; }
}

/// <summary>
/// Запрос на переезд в новую долину.
/// </summary>
public sealed record RelocateDto
{
    /// <summary>
    /// Долина, выбранная из <see cref="RelocationPlanDto.Valleys"/>.
    /// </summary>
    public int ValleyId { get; init; }

    /// <summary>
    /// Новое имя деревни.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> – оставить прежнее имя.
    /// </remarks>
    public string? VillageName { get; init; }
}
