using Domiki.Web.Data.Entities;

namespace Domiki.Web.Village.Models;

/// <summary>
/// Состояние переезда в новую долину: гейт, узелки памяти и лесенка перков.
/// </summary>
/// <remarks>
/// Считается в <see cref="RelocationManager.GetState"/> и едет в снимке состояния игры, поэтому держит только то,
/// что отвечается дешёвыми запросами. Сборы обоза (что едет, что остаётся, сколько узелков) считает
/// <see cref="RelocationManager.GetPlan"/> – их берут отдельным запросом при открытии раздела.
/// </remarks>
public class Relocation
{
    /// <summary>
    /// Обжитость деревни, с которой откроется ближайший переезд.
    /// </summary>
    public required int Threshold { get; init; }

    /// <summary>
    /// Нынешняя обжитость деревни.
    /// </summary>
    public required int Level { get; init; }

    /// <summary>
    /// Оценка срока до порога по нынешнему ходу деревни.
    /// </summary>
    /// <value>Сутки.</value>
    /// <remarks>
    /// <see langword="null"/> – ход ещё не с чего считать (деревня заведена до среза «Переезд», прожила меньше суток
    /// или обжитость нулевая) либо порог уже взят. Порог показывается игроку сроком, а число обжитости уходит
    /// в подсказку (GAMEDESIGN.md §3 Слой 4).
    /// </remarks>
    public required int? EstimatedDays { get; init; }

    /// <summary>
    /// Момент, раньше которого переехать нельзя из-за кулдауна.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> – кулдауна нет (игрок ещё не переезжал либо срок вышел).
    /// </remarks>
    public required DateTime? CooldownUntil { get; init; }

    /// <summary>
    /// Готов ли переезд прямо сейчас.
    /// </summary>
    /// <remarks>
    /// <see langword="false"/> – мешает то, что названо в <see cref="BlockReason"/>.
    /// </remarks>
    public required bool CanRelocate { get; init; }

    /// <summary>
    /// Что мешает переехать прямо сейчас.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> – ничего не мешает (<see cref="CanRelocate"/>).
    /// </remarks>
    public required string? BlockReason { get; init; }

    /// <summary>
    /// Остаток нетраченых узелков памяти.
    /// </summary>
    public required int Knots { get; init; }

    /// <summary>
    /// Число уже совершённых переездов.
    /// </summary>
    public required int RelocationCount { get; init; }

    /// <summary>
    /// Долина, в которой стоит нынешняя деревня.
    /// </summary>
    public required int ValleyId { get; init; }

    /// <summary>
    /// Название долины, в которой стоит нынешняя деревня.
    /// </summary>
    /// <remarks>
    /// Клиент назвать её сам не может: справочник долин едет только в сборах обоза, а стартовой долины в выборе нет
    /// вовсе (см. <see cref="RelocationValleys.Get"/>).
    /// </remarks>
    public required string ValleyName { get; init; }

    /// <summary>
    /// Лесенка перков со справочными ценами и купленными ступенями.
    /// </summary>
    public required Perk[] Perks { get; init; }
}

/// <summary>
/// Сборы обоза: что уедет, что останется, сколько узелков даст деревня и куда можно уехать.
/// </summary>
/// <remarks>
/// Считается в <see cref="RelocationManager.GetPlan"/> отдельным запросом при открытии раздела: обход склада,
/// двора и декора нужен только для диалога подтверждения, а не на каждом опросе состояния игры.
/// </remarks>
public class RelocationPlan
{
    /// <summary>
    /// Узелки памяти, которые деревня оставит на столбе при переезде прямо сейчас.
    /// </summary>
    public required int KnotsOnRelocate { get; init; }

    /// <summary>
    /// Что уедет с игроком и что останется здесь – сводка для первого шага подтверждения.
    /// </summary>
    public required RelocationSummary Summary { get; init; }

    /// <summary>
    /// Долины, из которых выбирают новое место.
    /// </summary>
    public required Valley[] Valleys { get; init; }
}

/// <summary>
/// Поимённая сводка переезда: числа для обеих колонок диалога подтверждения.
/// </summary>
/// <remarks>
/// Диалог перечисляет обе колонки с числами до подтверждения – это защита от «сброс сжёг неожидаемое»
/// (GAMEDESIGN_IMPL.md, три способа сделать игру хуже).
/// </remarks>
public class RelocationSummary
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
    /// Не больше <see cref="RelocationManager.GoldCarryCap"/>, излишек сгорает.
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
    /// Сколько построек останется вместе со всеми их уровнями.
    /// </summary>
    public required int Buildings { get; init; }

    /// <summary>
    /// Сколько монет будет в казне новой деревни.
    /// </summary>
    /// <remarks>
    /// Стартовые монеты плюс подъёмные (см. <see cref="PerkManager.GetStartingCoins"/>).
    /// </remarks>
    public required int StartingCoins { get; init; }
}

/// <summary>
/// Ступенчатый перк лесенки узелков памяти вместе с купленным игроком уровнем.
/// </summary>
public class Perk
{
    /// <summary>
    /// Перк лесенки.
    /// </summary>
    public required RelocationPerkType Type { get; init; }

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
/// Долина, в которой может встать новая деревня, – имя и вид, без модификатора местности.
/// </summary>
/// <remarks>
/// Механическую нагрузку в первой версии несут перки, долина остаётся косметикой (GAMEDESIGN.md §3 Слой 4).
/// </remarks>
public class Valley
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
/// <remarks>
/// Наружу отдаётся только число переездов и суммарная обжитость (GAMEDESIGN.md §3.7).
/// </remarks>
public class MemorialPost
{
    /// <summary>
    /// Прожитые деревни, свежие первыми.
    /// </summary>
    public required MemorialVillage[] Villages { get; init; }

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
    /// <see langword="null"/> – игрок завёл деревню до среза «Переезд», и первый день не записан.
    /// </remarks>
    public required DateTime? FirstDayDate { get; init; }
}

/// <summary>
/// Одна прожитая деревня на памятном столбе.
/// </summary>
public class MemorialVillage
{
    /// <summary>
    /// Имя прожитой деревни.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> – деревня так и осталась безымянной.
    /// </remarks>
    public required string? VillageName { get; init; }

    /// <summary>
    /// Индекс пиктограммы герба.
    /// </summary>
    public required int CrestIcon { get; init; }

    /// <summary>
    /// Индекс цвета герба.
    /// </summary>
    public required int CrestColor { get; init; }

    /// <summary>
    /// Долина, в которой стояла деревня.
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
