using System.ComponentModel.DataAnnotations;

namespace SMAIAXBackend.Application.DTOs;

public class MetadataCreateDto(DateTime validFrom, LocationDto? location, int? householdSize)
{
    [Required]
    public DateTime ValidFrom { get; set; } = validFrom;

    public LocationDto? Location { get; set; } = location;

    public int? HouseholdSize { get; set; } = householdSize;
}