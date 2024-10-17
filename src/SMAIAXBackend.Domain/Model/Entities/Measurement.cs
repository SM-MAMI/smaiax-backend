using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public class Measurement : IEqualityComparer<Measurement>
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

    public bool Equals(Measurement? x, Measurement? y)
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

    public int GetHashCode(Measurement obj)
    {
        return obj.Id.GetHashCode();
    }
}