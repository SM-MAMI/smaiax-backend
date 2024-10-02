namespace SMAIAXBackend.Domain.Model.ValueObjects;

public class UserId(Guid id) : ValueObject
{
    public Guid Id { get; } = id;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
    }
}