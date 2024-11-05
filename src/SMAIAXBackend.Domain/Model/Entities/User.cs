using System.Diagnostics.CodeAnalysis;

using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public sealed class User : IEquatable<User>
{
    public UserId Id { get; } = null!;
    public Name Name { get; } = null!;
    public string Email { get; } = null!;
    public TenantId TenantId { get; } = null!;

    public static User Create(UserId id, Name name, string email, TenantId tenantId)
    {
        return new User(id, name, email, tenantId);
    }

    // Needed by EF Core
    [ExcludeFromCodeCoverage]
    private User()
    {
    }

    private User(UserId id, Name name, string email, TenantId tenantId)
    {
        Id = id;
        Name = name;
        Email = email;
        TenantId = tenantId;
    }

    [ExcludeFromCodeCoverage]
    public bool Equals(User? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id.Equals(other.Id);
    }

    [ExcludeFromCodeCoverage]
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((User)obj);
    }

    [ExcludeFromCodeCoverage]
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}