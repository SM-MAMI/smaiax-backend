using System.ComponentModel.DataAnnotations;

using SMAIAXBackend.Domain.Model.Enums;

namespace SMAIAXBackend.Application.DTOs;

public class PolicyRequestCreateDto(
    bool isAutomaticContractingEnabled,
    MeasurementResolution measurementResolution,
    int minHouseHoldSize,
    int maxHouseHoldSize,
    List<LocationDto> locations,
    LocationResolution locationResolution,
    decimal maxPrice)
{
    [Required]
    public bool IsAutomaticContractingEnabled { get; set; } = isAutomaticContractingEnabled;
    [Required]
    public MeasurementResolution MeasurementResolution { get; set; } = measurementResolution;
    [Required]
    public int MinHouseHoldSize { get; set; } = minHouseHoldSize;
    [Required]
    public int MaxHouseHoldSize { get; set; } = maxHouseHoldSize;
    [Required]
    public List<LocationDto> Locations { get; set; } = locations;
    [Required]
    public LocationResolution LocationResolution { get; set; } = locationResolution;
    [Required]
    public decimal MaxPrice { get; set; } = maxPrice;
}