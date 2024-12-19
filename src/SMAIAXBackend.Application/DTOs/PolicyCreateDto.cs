using System.ComponentModel.DataAnnotations;

using SMAIAXBackend.Domain.Model.Enums;

namespace SMAIAXBackend.Application.DTOs;

public class PolicyCreateDto(string name,
    MeasurementResolution measurementResolution,
    LocationResolution locationResolution,
    decimal price,
    Guid smartMeterId)
{
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = name;
    [Required]
    public MeasurementResolution MeasurementResolution { get; set; } = measurementResolution;
    [Required]
    public LocationResolution LocationResolution { get; set; } = locationResolution;
    [Required]
    public decimal Price { get; set; } = price;
    [Required]
    public Guid SmartMeterId { get; set; } = smartMeterId;
}