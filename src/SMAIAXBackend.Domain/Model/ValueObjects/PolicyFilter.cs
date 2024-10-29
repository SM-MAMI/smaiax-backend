using System.Diagnostics.CodeAnalysis;

using SMAIAXBackend.Domain.Model.Enums;

namespace SMAIAXBackend.Domain.Model.ValueObjects;

public class PolicyFilter(
    MeasurementResolution measurementResolution,
    int minHouseHoldSize,
    int maxHouseHoldSize,
    List<Location> locations,
    LocationResolution locationResolution,
    decimal maxPrice) : ValueObject
{
    public MeasurementResolution MeasurementResolution { get; set; } = measurementResolution;
    public int MinHouseHoldSize { get; } = minHouseHoldSize;
    public int MaxHouseHoldSize { get; } = maxHouseHoldSize;
    public List<Location> Locations { get; set; } = locations;
    public LocationResolution LocationResolution { get; set; } = locationResolution;
    public decimal MaxPrice { get; set; } = maxPrice;

    [ExcludeFromCodeCoverage]
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return MeasurementResolution;
        yield return MinHouseHoldSize;
        yield return MaxHouseHoldSize;
        yield return Locations;
        yield return LocationResolution;
        yield return MaxPrice;
    }
}