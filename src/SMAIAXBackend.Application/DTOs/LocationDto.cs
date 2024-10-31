using SMAIAXBackend.Domain.Model.Enums;

namespace SMAIAXBackend.Application.DTOs;

public class LocationDto(string? streetName, string? city, string? state, string? country, Continent? continent)
{
    public string? StreetName { get; set;  } = streetName;
    public string? City { get; set;  } = city;
    public string? State { get; set;  } = state;
    public string? Country { get; set; } = country;
    public Continent? Continent { get; set;  } = continent;
}