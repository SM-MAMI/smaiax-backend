using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;

namespace SMAIAXBackend.Application.DTOs;

public class LocationDto(string? streetName, string? city, string? state, string? country, Continent? continent)
{
    public string? StreetName { get; set; } = streetName;
    public string? City { get; set; } = city;
    public string? State { get; set; } = state;
    public string? Country { get; set; } = country;
    public Continent? Continent { get; set; } = continent;

    public static LocationDto FromLocation(Location location)
    {
        return new LocationDto(location.StreetName, location.City, location.State, location.Country, location.Continent);
    }
}