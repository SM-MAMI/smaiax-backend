namespace SMAIAXBackend.Domain.Model.ValueObjects;

public class RefreshTokenId(Guid id) : ValueObject
{
    public Guid Id { get; } = id;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
    }
}