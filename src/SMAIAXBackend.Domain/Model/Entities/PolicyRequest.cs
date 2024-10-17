using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

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

    public static PolicyRequest DeepClone(PolicyRequest policyRequest)
    {
        var idCopy = new PolicyRequestId(policyRequest.Id.Id);
        var houseHoldSizesCopy = new List<int>(policyRequest.PolicyFilter.HouseHoldSizes);
        var locationsCopy = new List<Location>(policyRequest.PolicyFilter.Locations);
        var policyFilterCopy = new PolicyFilter(policyRequest.PolicyFilter.MeasurementResolution, houseHoldSizesCopy,
            locationsCopy, policyRequest.PolicyFilter.LocationResolution, policyRequest.PolicyFilter.MaxPrice);

        return new PolicyRequest(idCopy, policyRequest.IsAutomaticContractingEnabled, policyFilterCopy);
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