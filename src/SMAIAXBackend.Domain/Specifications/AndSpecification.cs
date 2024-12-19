namespace SMAIAXBackend.Domain.Specifications;

public class AndSpecification<T>(ISpecification<T> first, ISpecification<T> second) : ISpecification<T>
{
    public bool IsSatisfiedBy(T entity)
    {
        return first.IsSatisfiedBy(entity) && second.IsSatisfiedBy(entity);
    }
}