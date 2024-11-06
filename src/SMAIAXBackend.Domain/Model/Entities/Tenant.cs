using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public sealed class Tenant : IEquatable<Tenant>
{
    public TenantId Id { get; }
    // TODO: Move credentials to a secure store e.g. HashiCorp Vault
    public string DatabaseUsername { get; }
    public string DatabasePassword { get; }
    public string DatabaseName { get; }

    public static Tenant Create(TenantId id, string databaseUsername, string databasePassword, string databaseName)
    {
        return new Tenant(id, databaseUsername, databasePassword, databaseName);
    }
    
    // Needed by EF Core
    private Tenant()
    {
    }
    
    private Tenant(TenantId id, string databaseUsername, string databasePassword, string databaseName)
    {
        Id = id;
        DatabaseUsername = databaseUsername;
        DatabasePassword = databasePassword;
        DatabaseName = databaseName;
    }
    
    public bool Equals(Tenant? other)
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

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is Tenant other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}