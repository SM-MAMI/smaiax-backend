using System.Diagnostics.CodeAnalysis;

using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public sealed class Metadata : IEquatable<Metadata>
{
    public MetadataId Id { get; } = null!;
    public DateTime ValidFrom { get; }
    public Location? Location { get; } = null!;
    public int? HouseholdSize { get; }
    public SmartMeterId SmartMeterId { get; }
    public SmartMeter SmartMeter { get; } = null!;

    public static Metadata Create(
        MetadataId metadataId,
        DateTime validFrom,
        Location? location,
        int? householdSize,
        SmartMeterId smartMeterId)
    {
        return new Metadata(metadataId, validFrom, location, householdSize, smartMeterId);
    }

    // Needed by EF Core
    [ExcludeFromCodeCoverage]
    private Metadata()
    {
    }

    private Metadata(
        MetadataId metadataId,
        DateTime validFrom,
        Location? location,
        int? householdSize,
        SmartMeterId smartMeterId)
    {
        Id = metadataId;
        ValidFrom = validFrom;
        Location = location;
        HouseholdSize = householdSize;
        SmartMeterId = smartMeterId;
    }

    [ExcludeFromCodeCoverage]
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

        return Equals((Metadata)obj);
    }

    [ExcludeFromCodeCoverage]
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}