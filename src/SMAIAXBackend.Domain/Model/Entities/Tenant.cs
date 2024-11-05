using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public sealed class Tenant : IEquatable<Tenant>
{
    public TenantId Id { get; }
    public string Name { get; }
    public string DatabaseConnectionString { get; }

    public static Tenant Create(TenantId id, string name, string databaseConnectionString)
    {
        return new Tenant(id, name, databaseConnectionString);
    }
    
    // Needed by EF Core
    private Tenant()
    {
    }
    
    private Tenant(TenantId id, string name, string databaseConnectionString)
    {
        Id = id;
        Name = name;
        DatabaseConnectionString = databaseConnectionString;
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