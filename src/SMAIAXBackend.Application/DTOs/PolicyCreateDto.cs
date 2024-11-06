using System.ComponentModel.DataAnnotations;

using SMAIAXBackend.Domain.Model.Enums;

namespace SMAIAXBackend.Application.DTOs;

// TODO: Household siue and Location should be taken from the latest metadata
public class PolicyCreateDto(
    MeasurementResolution measurementResolution,
    int householdSize,
    LocationDto location,
    LocationResolution locationResolution,
    decimal price)
{
    [Required]
    public MeasurementResolution MeasurementResolution { get; set; } = measurementResolution;
    [Required]
    public int HouseholdSize { get; set; } = householdSize;
    [Required]
    public LocationDto Location { get; set; } = location;
    [Required]
    public LocationResolution LocationResolution { get; set; } = locationResolution;
    [Required]
    public decimal Price { get; set; } = price;
}