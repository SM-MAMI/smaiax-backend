using System.Diagnostics.CodeAnalysis;

using SMAIAXBackend.Domain.Model.Enums;

namespace SMAIAXBackend.Domain.Model.ValueObjects;

public class Location(string? streetName, string? city, string? state, string? country, Continent? continent)
    : ValueObject
{
    public string? StreetName { get; } = streetName;
    public string? City { get; } = city;
    public string? State { get; } = state;
    public string? Country { get; } = country;
    public Continent? Continent { get; } = continent;

    [ExcludeFromCodeCoverage]
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StreetName;
        yield return City;
        yield return State;
        yield return Country;
        yield return Continent;
    }
}