using SMAIAXBackend.Domain.Model.ValueObjects;

namespace SMAIAXBackend.Domain.Model.Entities;

// TODO: Implement ICloneable interface
public class PolicyRequest : IEqualityComparer<PolicyRequest>
{
    public PolicyRequestId Id { get; }
    public bool IsAutomaticContractingEnabled { get; }
    public PolicyFilter PolicyFilter { get; }

    public static PolicyRequest Create(
        PolicyRequestId id,
        bool isAutomaticContractingEnabled,
        PolicyFilter policyFilter)
    {
        return new PolicyRequest(id, isAutomaticContractingEnabled, policyFilter);
    }

    // Needed by EF Core
    private PolicyRequest()
    {
    }

    private PolicyRequest(PolicyRequestId id, bool isAutomaticContractingEnabled, PolicyFilter policyFilter)
    {
        Id = id;
        IsAutomaticContractingEnabled = isAutomaticContractingEnabled;
        PolicyFilter = policyFilter;
    }

    public bool Equals(PolicyRequest? x, PolicyRequest? y)
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

    public int GetHashCode(PolicyRequest obj)
    {
        return obj.Id.GetHashCode();
    }
}