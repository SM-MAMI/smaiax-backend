using SMAIAXBackend.Domain.Model.Entities;

namespace SMAIAXBackend.Application.DTOs;

public class SmartMeterDto(Guid id, string name, List<MetadataDto> metadata)
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    public List<MetadataDto> Metadata { get; set; } = metadata;

    public static SmartMeterDto FromSmartMeter(SmartMeter smartMeter)
    {
        return new SmartMeterDto(smartMeter.Id.Id, smartMeter.Name, smartMeter.Metadata.Select(MetadataDto.FromMetadata).ToList());
    }
}