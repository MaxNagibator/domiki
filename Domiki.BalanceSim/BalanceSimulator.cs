namespace Domiki.BalanceSim;

public sealed class BalanceSimulator
{
    private readonly SimulationData _data;

    public BalanceSimulator(SimulationData data)
    {
        _data = data;
    }

    /// <summary>
    /// Прогоняет три сценария по 7 сидов.
    /// </summary>
    /// <param name="profileNeighborId">Сосед, чей уклад деревни принят на старте прогона; <see langword="null"/> – без уклада.</param>
    /// <returns>Отчёт по трём сценариям.</returns>
    public SimulationReport Run(int? profileNeighborId = null)
    {
        var runs = new Dictionary<ScenarioKind, List<SimulationRunResult>>();
        foreach (var scenario in new[] { ScenarioKind.Casual, ScenarioKind.Optimal, ScenarioKind.Extreme })
        {
            runs[scenario] = Enumerable.Range(1, 7)
                .Select(seed => new SimulationRun(_data, scenario, seed, profileNeighborId: profileNeighborId).Run())
                .ToList();
        }

        return new SimulationReport(runs);
    }

    public IReadOnlyList<SimulationRunResult> RunFtue()
    {
        return Enumerable.Range(1, 7)
            .Select(seed => new SimulationRun(_data, ScenarioKind.Casual, seed, ftue: true).Run())
            .ToArray();
    }
}
