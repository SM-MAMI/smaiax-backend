using System.ComponentModel.DataAnnotations;

using SMAIAXBackend.Domain.Model.Entities;

namespace SMAIAXBackend.Application.DTOs;

public class MetadataDto(Guid id, DateTime validFrom, LocationDto location, int householdSize)
{
    [Required]
    public Guid Id { get; set; } = id;

    [Required]
    public DateTime ValidFrom { get; set; } = validFrom;

    [Required]
    public LocationDto Location { get; set; } = location;

    [Required]
    public int HouseholdSize { get; set; } = householdSize;

    public static MetadataDto FromMetadata(Metadata metadata)
    {
        return new MetadataDto(metadata.Id.Id, metadata.ValidFrom, LocationDto.FromLocation(metadata.Location), metadata.HouseholdSize);
    }
}