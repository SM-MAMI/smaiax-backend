using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public sealed class Metadata : IEquatable<Metadata>
{
    public MetadataId Id { get; } = null!;
    public DateTime CreatedAt { get; }
    public Location Location { get; } = null!;
    public int HouseholdSize { get; }

    public static Metadata Create(MetadataId metadataId, DateTime createdAt, Location location, int householdSize)
    {
        return new Metadata(metadataId, createdAt, location, householdSize);
    }

    // Needed by EF Core
    private Metadata()
    {
    }

    private Metadata(MetadataId metadataId, DateTime createdAt, Location location, int householdSize)
    {
        Id = metadataId;
        CreatedAt = createdAt;
        Location = location;
        HouseholdSize = householdSize;
    }

    public bool Equals(Metadata? other)
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

        return Equals((Metadata)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}