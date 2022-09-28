// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Specifications;

[Obsolete("BaseSpecification has expired, please use SpecificationBase")]
public abstract class BaseSpecification<T> : SpecificationBase<T>
{
    protected BaseSpecification(Expression<Func<T, bool>> whereExpression)
        : base(whereExpression)
    {
    }
}

public abstract class SpecificationBase<T> : ISpecification<T>
{
    protected SpecificationBase(Expression<Func<T, bool>> whereExpression)
    {
        WhereExpression = whereExpression;
    }

    public Expression<Func<T, bool>> WhereExpression { get; }

    public List<Expression<Func<T, object>>> Includes { get; } = new();

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
