namespace SMAIAXBackend.Domain.Model.ValueObjects;

public class MeasurementData(
    double positiveActivePower,
    double positiveActiveEnergyTotal,
    double negativeActivePower,
    double negativeActiveEnergyTotal,
    double reactiveEnergyQuadrant1Total,
    double reactiveEnergyQuadrant3Total,
    double totalPower,
    double currentPhase1,
    double voltagePhase1,
    double currentPhase2,
    double voltagePhase2,
    double currentPhase3,
    double voltagePhase3,
    string uptime,
    DateTime timestamp) : ValueObject
{
    public double PositiveActivePower { get; set; } = positiveActivePower;
    public double PositiveActiveEnergyTotal { get; set; } = positiveActiveEnergyTotal;
    public double NegativeActivePower { get; set; } = negativeActivePower;
    public double NegativeActiveEnergyTotal { get; set; } = negativeActiveEnergyTotal;
    public double ReactiveEnergyQuadrant1Total { get; set; } = reactiveEnergyQuadrant1Total;
    public double ReactiveEnergyQuadrant3Total { get; set; } = reactiveEnergyQuadrant3Total;
    public double TotalPower { get; set; } = totalPower;
    public double CurrentPhase1 { get; set; } = currentPhase1;
    public double VoltagePhase1 { get; set; } = voltagePhase1;
    public double CurrentPhase2 { get; set; } = currentPhase2;
    public double VoltagePhase2 { get; set; } = voltagePhase2;
    public double CurrentPhase3 { get; set; } = currentPhase3;
    public double VoltagePhase3 { get; set; } = voltagePhase3;
    public string Uptime { get; set; } = uptime;
    public DateTime Timestamp { get; set; } = timestamp;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PositiveActivePower;
        yield return PositiveActiveEnergyTotal;
        yield return NegativeActivePower;
        yield return NegativeActiveEnergyTotal;
        yield return ReactiveEnergyQuadrant1Total;
        yield return ReactiveEnergyQuadrant3Total;
        yield return TotalPower;
        yield return CurrentPhase1;
        yield return VoltagePhase1;
        yield return CurrentPhase2;
        yield return VoltagePhase2;
        yield return CurrentPhase3;
        yield return VoltagePhase3;
        yield return Uptime;
        yield return Timestamp;
    }
}