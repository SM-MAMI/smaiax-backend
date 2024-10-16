using System.Globalization;

namespace SMAIAXBackend.Domain.Model.ValueObjects;

public class Location(string? streetName, string? city, string? state, RegionInfo? country, Continent? continent)
    : ValueObject
{
    public string? StreetName { get; } = streetName;
    public string? City { get; } = city;
    public string? State { get; } = state;
    public RegionInfo? Country { get; } = country;
    public Continent? Continent { get; } = continent;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StreetName;
        yield return City;
        yield return State;
        yield return Country;
        yield return Continent;
    }
}