using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public sealed class Contract : IEquatable<Contract>
{
    public ContractId Id { get; } = null!;
    public DateTime CreatedAt { get; }
    public PolicyId PolicyId { get; }
    public Policy PolicyCopy { get; } = null!;
    public PolicyRequestId PolicyRequestId { get; }
    public PolicyRequest PolicyRequestCopy { get; } = null!;

    private static Contract Create(
        ContractId id,
        DateTime createdAt,
        PolicyId policyId,
        Policy policy,
        PolicyRequestId policyRequestId,
        PolicyRequest policyRequest)
    {
        var policyCopy = Policy.DeepClone(policy);
        var policyRequestCopy = PolicyRequest.DeepClone(policyRequest);

        return new Contract(id, createdAt, policyId, policyCopy, policyRequestId, policyRequestCopy);
    }

    // Needed by EF Core
    private Contract()
    {
    }

    private Contract(
        ContractId id,
        DateTime createdAt,
        PolicyId policyId,
        Policy policyCopy,
        PolicyRequestId policyRequestId,
        PolicyRequest policyRequestCopy)
    {
        Id = id;
        CreatedAt = createdAt;
        PolicyId = policyId;
        PolicyCopy = policyCopy;
        PolicyRequestId = policyRequestId;
        PolicyRequestCopy = policyRequestCopy;
    }

    public bool Equals(Contract? other)
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

        return Equals((Contract)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}