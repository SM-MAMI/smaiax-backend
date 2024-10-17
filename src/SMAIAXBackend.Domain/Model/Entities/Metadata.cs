using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public class Metadata : IEqualityComparer<Metadata>
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

    public bool Equals(Metadata? x, Metadata? y)
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

    public int GetHashCode(Metadata obj)
    {
        return obj.Id.GetHashCode();
    }
}