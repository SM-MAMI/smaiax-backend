using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public sealed class SmartMeter : IEquatable<SmartMeter>
{
    public SmartMeterId Id { get; } = null!;
    public string Name { get; } = null!;
    public List<Metadata> Metadata { get; }
    public UserId UserId { get; }
    public List<PolicyId> PolicyIds { get; }
    public List<MeasurementId> MeasurementIds { get; }

    public static SmartMeter Create(SmartMeterId smartMeterId, string name, UserId userId)
    {
        return new SmartMeter(smartMeterId, name, userId);
    }

    // Needed by EF Core
    private SmartMeter()
    {
    }

    private SmartMeter(SmartMeterId smartMeterId, string name, UserId userId)
    {
        Id = smartMeterId;
        Name = name;
        Metadata = [];
        UserId = userId;
        PolicyIds = [];
        MeasurementIds = [];
    }

    public bool Equals(SmartMeter? other)
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

        return Equals((SmartMeter)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}