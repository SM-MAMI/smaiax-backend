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
    public bool IsAutomaticContractingEnabled { get; set; } = isAutomaticContractingEnabled;
    public MeasurementResolution MeasurementResolution { get; set; } = measurementResolution;
    public int MinHouseHoldSize { get; set; } = minHouseHoldSize;
    public int MaxHouseHoldSize { get; set; } = maxHouseHoldSize;
    public List<LocationDto> Locations { get; set; } = locations;
    public LocationResolution LocationResolution { get; set; } = locationResolution;
    public decimal MaxPrice { get; set; } = maxPrice;
}