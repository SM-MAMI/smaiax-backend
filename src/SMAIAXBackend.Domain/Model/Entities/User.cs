using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public class User : IEqualityComparer<User>
{
    public UserId Id { get; } = null!;
    public Name Name { get; } = null!;
    public string Email { get; } = null!;

    public static User Create(UserId id, Name name, string email)
    {
        return new User(id, name, email);
    }

    // Needed by EF Core
    private User()
    {
    }

    private User(UserId id, Name name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }

    public bool Equals(User? x, User? y)
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

    public int GetHashCode(User obj)
    {
        return obj.Id.GetHashCode();
    }
}