using SMAIAXBackend.Domain.Model.ValueObjects;

namespace SMAIAXBackend.Domain.Model.Entities;

public class Policy : IEqualityComparer<Policy>
{
    public PolicyId Id { get; } = null!;
    public MeasurementResolution MeasurementResolution { get; }
    public int HouseholdSize { get; }
    public Location Location { get; } = null!;
    public LocationResolution LocationResolution { get; }
    public decimal Price { get; }

    public static Policy Create(
        PolicyId id,
        MeasurementResolution measurementResolution,
        int householdSize,
        Location location,
        LocationResolution locationResolution,
        decimal price)
    {
        return new Policy(id, measurementResolution, householdSize, location, locationResolution, price);
    }

    public static Policy DeepClone(Policy policy)
    {
        var policyIdCopy = new PolicyId(policy.Id.Id);
        var locationCopy = new Location(policy.Location.StreetName, policy.Location.City, policy.Location.State,
            policy.Location.Country, policy.Location.Continent);

        return new Policy(policyIdCopy, policy.MeasurementResolution, policy.HouseholdSize, locationCopy,
            policy.LocationResolution, policy.Price);
    }

    // Needed by EF Core
    private Policy()
    {
    }

    private Policy(
        PolicyId id,
        MeasurementResolution measurementResolution,
        int householdSize,
        Location location,
        LocationResolution locationResolution,
        decimal price)
    {
        Id = id;
        MeasurementResolution = measurementResolution;
        HouseholdSize = householdSize;
        Location = location;
        LocationResolution = locationResolution;
        Price = price;
    }

    public bool Equals(Policy? x, Policy? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null)
        {
            return false;
        }

        if (y is null)
        {
            return false;
        }

        if (x.GetType() != y.GetType())
        {
            return false;
        }

        return x.Id.Equals(y.Id);
    }

    public int GetHashCode(Policy obj)
    {
        return obj.Id.GetHashCode();
    }
}