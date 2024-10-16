using SMAIAXBackend.Domain.Model.ValueObjects;

namespace SMAIAXBackend.Domain.Model.Entities;

public class Contract : IEqualityComparer<Contract>
{
    public ContractId Id { get; } = null!;
    public DateTime CreatedAt { get; }
    public Policy PolicyCopy { get; } = null!;
    public PolicyRequest PolicyRequestCopy { get; } = null!;

    private static Contract Create(
        ContractId id,
        DateTime createdAt,
        Policy policy,
        PolicyRequest policyRequest)
    {
        var policyCopy = Policy.DeepClone(policy);
        var policyRequestCopy = PolicyRequest.DeepClone(policyRequest);

        return new Contract(id, createdAt, policyCopy, policyRequestCopy);
    }

    // Needed by EF Core
    private Contract()
    {
    }

    private Contract(ContractId id, DateTime createdAt, Policy policyCopy, PolicyRequest policyRequestCopy)
    {
        Id = id;
        CreatedAt = createdAt;
        PolicyCopy = policyCopy;
        PolicyRequestCopy = policyRequestCopy;
    }

    public bool Equals(Contract? x, Contract? y)
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

    public int GetHashCode(Contract obj)
    {
        return obj.Id.GetHashCode();
    }
}