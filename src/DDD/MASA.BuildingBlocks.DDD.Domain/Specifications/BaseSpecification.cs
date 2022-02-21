namespace MASA.BuildingBlocks.DDD.Domain.Specifications;
public abstract class BaseSpecification<T> : ISpecification<T>
{
    public BaseSpecification(Expression<Func<T, bool>> whereExpression)
    {
        WhereExpression = whereExpression;
    }
    public Expression<Func<T, bool>> WhereExpression { get; }

    public List<Expression<Func<T, object>>> Includes { get; } =
                                           new List<Expression<Func<T, object>>>();

    public List<string> IncludeStrings { get; } = new List<string>();

    public abstract bool IsSatisfiedBy(T obj);

    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected virtual void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }
}
