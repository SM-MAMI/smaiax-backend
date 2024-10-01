using System.Collections;

namespace SMAIAXBackend.Domain.Model.ValueObjects;

public class Name(string firstName, string lastName) : IEqualityComparer<Name>
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;

    public bool Equals(Name? x, Name? y)
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

        return x.FirstName == y.FirstName && x.LastName == y.LastName;
    }

    public int GetHashCode(Name obj)
    {
        return HashCode.Combine(obj.FirstName, obj.LastName);
    }
}