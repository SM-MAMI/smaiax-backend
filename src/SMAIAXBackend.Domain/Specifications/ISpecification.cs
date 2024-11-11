namespace SMAIAXBackend.Domain.Specifications;

public interface ISpecification<in T>
{
    bool IsSatisfiedBy(T entity);
}