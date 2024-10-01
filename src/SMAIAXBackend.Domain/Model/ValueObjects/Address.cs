namespace SMAIAXBackend.Domain.Model.ValueObjects;

public class Address(string street, string city, string state, string zipCode, string country) : IEqualityComparer<Address>
{
    public string Street { get; } = street;
    public string City { get; } = city;
    public string State { get; } = state;
    public string ZipCode { get; } = zipCode;
    public string Country { get; } = country;


    public bool Equals(Address? x, Address? y)
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

        return x.Street == y.Street && x.City == y.City && x.State == y.State && x.ZipCode == y.ZipCode && x.Country == y.Country;
    }

    public int GetHashCode(Address obj)
    {
        return HashCode.Combine(obj.Street, obj.City, obj.State, obj.ZipCode, obj.Country);
    }
}