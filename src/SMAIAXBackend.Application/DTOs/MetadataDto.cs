using SMAIAXBackend.Domain.Model.Entities;

namespace SMAIAXBackend.Application.DTOs;

public class MetadataDto(Guid id, DateTime validFrom, LocationDto location, int householdSize)
{
    public Guid Id { get; set; } = id;
    public DateTime ValidFrom { get; set; } = validFrom;
    public LocationDto Location { get; set; } = location;
    public int HouseholdSize { get; set; } = householdSize;

    public static MetadataDto FromMetadata(Metadata metadata)
    {
        return new MetadataDto(metadata.Id.Id, metadata.ValidFrom, LocationDto.FromLocation(metadata.Location), metadata.HouseholdSize);
    }
}