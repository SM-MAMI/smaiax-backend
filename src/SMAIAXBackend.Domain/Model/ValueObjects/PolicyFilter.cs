namespace SMAIAXBackend.Domain.Model.ValueObjects;

public class PolicyFilter(
    MeasurementResolution measurementResolutions,
    List<int> houseHoldSizes,
    List<Location> locations,
    LocationResolution locationResolution,
    decimal maxPrice) : ValueObject
{
    public MeasurementResolution MeasurementResolution { get; set; } = measurementResolutions;
    public List<int> HouseHoldSizes { get; set; } = houseHoldSizes;
    public List<Location> Locations { get; set; } = locations;
    public LocationResolution LocationResolution { get; set; } = locationResolution;
    public decimal MaxPrice { get; set; } = maxPrice;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return MeasurementResolution;
        yield return HouseHoldSizes;
        yield return Locations;
        yield return LocationResolution;
        yield return MaxPrice;
    }
}