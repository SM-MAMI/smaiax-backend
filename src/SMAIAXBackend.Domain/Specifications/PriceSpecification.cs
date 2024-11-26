using SMAIAXBackend.Domain.Model.Entities;

namespace SMAIAXBackend.Domain.Specifications;

public class PriceSpecification(decimal maxPrice) : ISpecification<Policy>
{
    public bool IsSatisfiedBy(Policy policy)
    {
        return policy.Price <= maxPrice;
    }
}