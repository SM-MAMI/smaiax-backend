using System.ComponentModel.DataAnnotations;

namespace SMAIAXBackend.Application.DTOs;

public class SmartMeterCreateDto(string name, MetadataCreateDto? metadata)
{
    [Required]
    public string Name { get; set; } = name;

    public MetadataCreateDto? Metadata { get; set; } = metadata;
}