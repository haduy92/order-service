using Ardalis.Specification;

namespace Domain.Specifications;

public abstract class BaseSpecification<T> : Specification<T>
{
}

public abstract class BaseSpecification<T, TResult> : Specification<T, TResult>
{
}
