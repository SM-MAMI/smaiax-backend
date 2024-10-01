namespace SMAIAXBackend.Domain.Model.ValueObjects;

public class UserId(Guid id) : IEqualityComparer<UserId>
{
    public Guid Id { get; } = id;

    public bool Equals(UserId? x, UserId? y)
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

        return x.Id.Equals(y.Id);
    }

    public int GetHashCode(UserId obj)
    {
        return obj.Id.GetHashCode();
    }
}