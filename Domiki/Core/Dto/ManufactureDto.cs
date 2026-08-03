namespace Domiki.Web.Core.Dto;

/// <summary>
/// Запущенное в домике производство по рецепту.
/// </summary>
public sealed record ManufactureDto
{
    /// <summary>
    /// Идентификатор производства.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Момент завершения производства.
    /// </summary>
    /// <value>Момент в UTC.</value>
    public required DateTime FinishDate { get; init; }

    /// <summary>
    /// Фактическая длительность производства.
    /// </summary>
    /// <value>Секунды.</value>
    /// <remarks>
    /// Учитывает скорость трудяг и стартовый бонус «нетронутые залежи», поэтому отличается от базовой
    /// <see cref="Reference.Dto.ReceiptDto.DurationSeconds"/>. Вместе с <see cref="FinishDate"/> задаёт долю пройденного пути.
    /// </remarks>
    public required int DurationSeconds { get; init; }

    /// <summary>
    /// Сколько трудяг занято в этом производстве.
    /// </summary>
    public required int PlodderCount { get; init; }

    /// <summary>
    /// Рецепт, по которому идёт производство – ссылка на <see cref="Reference.Dto.ReceiptDto.Id"/>.
    /// </summary>
    public required int ReceiptId { get; init; }

    /// <summary>
    /// Перезапускать ли рецепт автоматически по завершении.
    /// </summary>
    /// <remarks>
    /// Работает, пока хватает ресурсов и свободных трудяг.
    /// </remarks>
    public required bool AutoRepeat { get; init; }

    /// <summary>
    /// Мера наряда: ресурс, по запасу которого наряд снимается сам.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> – меры нет, наряд идёт, пока хватает припасов.
    /// </remarks>
    public int? MeasureResourceTypeId { get; init; }

    /// <summary>
    /// Мера наряда: сколько единиц ресурса нужно набрать.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> – меры нет.
    /// </remarks>
    public int? MeasureValue { get; init; }
}
