using SMAIAXBackend.Domain.Model.Entities;

namespace SMAIAXBackend.Application.DTOs;

public class MeasurementRawDto(
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

    public static MeasurementRawDto FromMeasurement(Measurement measurement)
    {
        return new MeasurementRawDto(measurement.PositiveActivePower, measurement.PositiveActiveEnergyTotal,
            measurement.NegativeActivePower, measurement.NegativeActiveEnergyTotal,
            measurement.ReactiveEnergyQuadrant1Total, measurement.ReactiveEnergyQuadrant3Total, measurement.TotalPower,
            measurement.CurrentPhase1, measurement.VoltagePhase1,
            measurement.CurrentPhase2, measurement.VoltagePhase2, measurement.CurrentPhase3, measurement.VoltagePhase3,
            measurement.Uptime, measurement.Timestamp);
    }
}