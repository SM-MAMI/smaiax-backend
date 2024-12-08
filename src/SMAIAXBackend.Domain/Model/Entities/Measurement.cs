using System.Diagnostics.CodeAnalysis;

using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public sealed class Measurement
{
    // Needed by EF Core
    [ExcludeFromCodeCoverage]
    private Measurement()
    {
    }


    public Measurement(SmartMeterId smartMeterId, double positiveActivePower, double positiveActiveEnergyTotal,
        double negativeActivePower, double negativeActiveEnergyTotal, double reactiveEnergyQuadrant1Total,
        double reactiveEnergyQuadrant3Total, double totalPower, double currentPhase1, double voltagePhase1,
        double currentPhase2, double voltagePhase2, double currentPhase3, double voltagePhase3, string uptime,
        DateTime timestamp)
    {
        SmartMeterId = smartMeterId;
        PositiveActivePower = positiveActivePower;
        PositiveActiveEnergyTotal = positiveActiveEnergyTotal;
        NegativeActivePower = negativeActivePower;
        NegativeActiveEnergyTotal = negativeActiveEnergyTotal;
        ReactiveEnergyQuadrant1Total = reactiveEnergyQuadrant1Total;
        ReactiveEnergyQuadrant3Total = reactiveEnergyQuadrant3Total;
        TotalPower = totalPower;
        CurrentPhase1 = currentPhase1;
        VoltagePhase1 = voltagePhase1;
        CurrentPhase2 = currentPhase2;
        VoltagePhase2 = voltagePhase2;
        CurrentPhase3 = currentPhase3;
        VoltagePhase3 = voltagePhase3;
        Uptime = uptime;
        Timestamp = timestamp;
    }

    public double PositiveActivePower { get; set; }
    public double PositiveActiveEnergyTotal { get; set; }
    public double NegativeActivePower { get; set; }
    public double NegativeActiveEnergyTotal { get; set; }
    public double ReactiveEnergyQuadrant1Total { get; set; }
    public double ReactiveEnergyQuadrant3Total { get; set; }
    public double TotalPower { get; set; }
    public double CurrentPhase1 { get; set; }
    public double VoltagePhase1 { get; set; }
    public double CurrentPhase2 { get; set; }
    public double VoltagePhase2 { get; set; }
    public double CurrentPhase3 { get; set; }
    public double VoltagePhase3 { get; set; }
    public string Uptime { get; set; }
    public DateTime Timestamp { get; set; }
    public SmartMeterId SmartMeterId { get; } = null!;

    public static Measurement Create(
        SmartMeterId smartMeterId,
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
        DateTime timestamp)
    {
        return new Measurement(smartMeterId, positiveActivePower, positiveActiveEnergyTotal, negativeActivePower,
            negativeActiveEnergyTotal, reactiveEnergyQuadrant1Total,
            reactiveEnergyQuadrant3Total, totalPower, currentPhase1, voltagePhase1, currentPhase2, voltagePhase2,
            currentPhase3, voltagePhase3, uptime, timestamp);
    }
}