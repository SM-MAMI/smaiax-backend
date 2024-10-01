using SMAIAXBackend.Domain.Model.ValueObjects;

namespace SMAIAXBackend.Domain.Model.Entities;

public class User : IEqualityComparer<User> 
{
    public UserId Id { get; }
    public Name Name { get; }
    public Address Address { get; }
    public string Email { get; }

    public static User Create(UserId id, Name name, Address address, string email)
    {
        return new User(id, name, address, email);
    }
    
    private User(UserId id, Name name, Address address, string email)
    {
        Address = address;
        Name = name;
        Id = id;
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