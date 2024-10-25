using SMAIAXBackend.Domain.Model.RelationshipHelpers;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public sealed class SmartMeter : IEquatable<SmartMeter>
{
    public SmartMeterId Id { get; } = null!;
    public string Name { get; } = null!;
    public List<Metadata> Metadata { get; }
    public UserId UserId { get; }
    public List<PolicySmartMeter> Policies { get; }

    public static SmartMeter Create(SmartMeterId smartMeterId, string name, UserId userId)
    {
        var metadata = new List<Metadata>();
        var policies = new List<PolicySmartMeter>();
        return new SmartMeter(smartMeterId, name, metadata, userId, policies);
    }

    // Needed by EF Core
    private SmartMeter()
    {
    }

    private SmartMeter(SmartMeterId smartMeterId, string name, List<Metadata> metadata, UserId userId, List<PolicySmartMeter> policies)
    {
        Id = smartMeterId;
        Name = name;
        Metadata = metadata;
        UserId = userId;
        Policies = policies;
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