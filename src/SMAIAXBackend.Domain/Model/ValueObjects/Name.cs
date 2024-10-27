using System.Diagnostics.CodeAnalysis;

namespace SMAIAXBackend.Domain.Model.ValueObjects;

public class Name(string firstName, string lastName) : ValueObject
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;

    [ExcludeFromCodeCoverage]
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
}