using System.ComponentModel.DataAnnotations;

using SMAIAXBackend.Domain.Model.Entities;

namespace SMAIAXBackend.Application.DTOs;

public class SmartMeterDto(Guid id, string name, List<MetadataDto> metadata)
{
    [Required]
    public Guid Id { get; set; } = id;

    [Required]
    public string Name { get; set; } = name;

    [Required]
    public List<MetadataDto> Metadata { get; set; } = metadata;

    public static SmartMeterDto FromSmartMeter(SmartMeter smartMeter)
    {
        return new SmartMeterDto(smartMeter.Id.Id, smartMeter.Name, smartMeter.Metadata.Select(MetadataDto.FromMetadata).ToList());
    }
}