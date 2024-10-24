using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public sealed class Policy : IEquatable<Policy>
{
    public PolicyId Id { get; } = null!;
    public MeasurementResolution MeasurementResolution { get; }
    public int HouseholdSize { get; }
    public Location Location { get; } = null!;
    public LocationResolution LocationResolution { get; }
    public decimal Price { get; }
    public PolicyState State { get; }
    public UserId UserId { get; }

    public static Policy Create(
        PolicyId id,
        MeasurementResolution measurementResolution,
        int householdSize,
        Location location,
        LocationResolution locationResolution,
        decimal price,
        UserId userId)
    {
        return new Policy(id, measurementResolution, householdSize, location, locationResolution, price, userId);
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
        decimal price,
        UserId userId)
    {
        Id = id;
        MeasurementResolution = measurementResolution;
        HouseholdSize = householdSize;
        Location = location;
        LocationResolution = locationResolution;
        Price = price;
        State = PolicyState.Active;
        UserId = userId;
    }

    public bool Equals(Policy? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((Policy)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}