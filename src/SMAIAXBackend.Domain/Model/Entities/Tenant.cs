using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public sealed class Tenant : IEquatable<Tenant>
{
    public TenantId Id { get; } = null!;
    public string VaultRoleName { get; }
    public string DatabaseName { get; }

    public static Tenant Create(TenantId id, string vaulRoleName, string databaseName)
    {
        return new Tenant(id, vaulRoleName, databaseName);
    }

    // Needed by EF Core
    private Tenant()
    {
    }

    private Tenant(TenantId id, string vaultRoleName, string databaseName)
    {
        Id = id;
        VaultRoleName = vaultRoleName;
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