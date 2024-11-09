using System.Diagnostics.CodeAnalysis;

using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public sealed class Policy : IEquatable<Policy>
{
    public PolicyId Id { get; } = null!;
    public MeasurementResolution MeasurementResolution { get; }
    public LocationResolution LocationResolution { get; }
    public decimal Price { get; }
    public PolicyState State { get; }
    public UserId UserId { get; }
    public SmartMeterId SmartMeterId { get; }

    public static Policy Create(
        PolicyId id,
        MeasurementResolution measurementResolution,
        LocationResolution locationResolution,
        decimal price,
        UserId userId,
        SmartMeterId smartMeterId)
    {
        return new Policy(id, measurementResolution, locationResolution, price, userId, smartMeterId);
    }

    // Needed by EF Core
    [ExcludeFromCodeCoverage]
    private Policy()
    {
    }

    private Policy(
        PolicyId id,
        MeasurementResolution measurementResolution,
        LocationResolution locationResolution,
        decimal price,
        UserId userId,
        SmartMeterId smartMeterId)
    {
        Id = id;
        MeasurementResolution = measurementResolution;
        LocationResolution = locationResolution;
        Price = price;
        State = PolicyState.Active;
        UserId = userId;
        SmartMeterId = smartMeterId;
    }

    [ExcludeFromCodeCoverage]
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

    [ExcludeFromCodeCoverage]
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

    [ExcludeFromCodeCoverage]
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}