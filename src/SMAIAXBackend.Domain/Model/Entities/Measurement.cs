using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public sealed class Measurement : IEquatable<Measurement>
{
    public MeasurementId Id { get; } = null!;
    public DateTime Timestamp { get; }
    public MeasurementData Data { get; } = null!;

    public static Measurement Create(MeasurementId id, DateTime timestamp, MeasurementData data)
    {
        return new Measurement(id, timestamp, data);
    }

    // Needed by EF Core
    private Measurement()
    {
    }

    private Measurement(MeasurementId id, DateTime timestamp, MeasurementData data)
    {
        Id = id;
        Timestamp = timestamp;
        Data = data;
    }

    public bool Equals(Measurement? other)
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

        return Equals((Measurement)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}