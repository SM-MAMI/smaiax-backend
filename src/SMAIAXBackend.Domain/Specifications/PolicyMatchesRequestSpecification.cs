using SMAIAXBackend.Domain.Model.Entities;

namespace SMAIAXBackend.Domain.Specifications;

public class PolicyMatchesRequestSpecification(PolicyRequest policyRequest) : ISpecification<Policy>
{
    private readonly List<ISpecification<Policy>> _specifications =
    [
        new PriceSpecification(policyRequest.PolicyFilter.MaxPrice),
        new MeasurementResolutionSpecification(policyRequest.PolicyFilter.MeasurementResolution)
    ];

    public bool IsSatisfiedBy(Policy policy)
    {
        return _specifications.TrueForAll(specification => specification.IsSatisfiedBy(policy));
    }
}