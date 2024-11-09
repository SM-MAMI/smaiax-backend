using SMAIAXBackend.Domain.Model.Entities;

namespace SMAIAXBackend.Application.DTOs;

public class SmartMeterOverviewDto(Guid id, string name, int metadataCount, int policyCount)
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    public int MetadataCount { get; set; } = metadataCount;
    public int PolicyCount { get; set; } = policyCount;

    public static SmartMeterOverviewDto FromSmartMeter(SmartMeter smartMeter, List<Policy> policies)
    {
        return new SmartMeterOverviewDto(smartMeter.Id.Id, smartMeter.Name,
            smartMeter.Metadata.Count, policies.Count);
    }
}