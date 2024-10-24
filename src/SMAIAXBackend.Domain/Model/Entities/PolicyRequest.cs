using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public sealed class PolicyRequest : IEquatable<PolicyRequest>
{
    public PolicyRequestId Id { get; }
    public bool IsAutomaticContractingEnabled { get; }
    public PolicyFilter PolicyFilter { get; }
    public PolicyRequestState State { get; }
    public UserId UserId { get; }

    public static PolicyRequest Create(
        PolicyRequestId id,
        bool isAutomaticContractingEnabled,
        PolicyFilter policyFilter,
        UserId userId)
    {
        return new PolicyRequest(id, isAutomaticContractingEnabled, policyFilter, userId);
    }

    // Needed by EF Core
    private PolicyRequest()
    {
    }

    private PolicyRequest(
        PolicyRequestId id,
        bool isAutomaticContractingEnabled,
        PolicyFilter policyFilter,
        UserId userId)
    {
        Id = id;
        IsAutomaticContractingEnabled = isAutomaticContractingEnabled;
        PolicyFilter = policyFilter;
        UserId = userId;
        State = PolicyRequestState.Pending;
    }

    public bool Equals(PolicyRequest? other)
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

        return Equals((PolicyRequest)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}