namespace SMAIAXBackend.Domain.Specifications;

public class BaseSpecification<T> : ISpecification<T>
{
    public bool IsSatisfiedBy(T entity) => true;
}