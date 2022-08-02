// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Specifications;

/// <summary>
/// Reference from
/// https://docs.microsoft.com/zh-cn/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-implementation-entity-framework-core
/// https://martinfowler.com/apsupp/spec.pdf
/// </summary>
public interface ISpecification<T>
{
    bool IsSatisfiedBy(T obj);

    Expression<Func<T, bool>> WhereExpression { get; }

    List<Expression<Func<T, object>>> Includes { get; }

    List<string> IncludeStrings { get; }
}
