namespace SMAIAXBackend.Domain.Model.ValueObjects;

public class Address(string street, string city, string state, string zipCode, string country) : ValueObject
{
    public string Street { get; } = street;
    public string City { get; } = city;
    public string State { get; } = state;
    public string ZipCode { get; } = zipCode;
    public string Country { get; } = country;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return ZipCode;
        yield return Country;
    }
}