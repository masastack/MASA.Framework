// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.ReadWriteSplitting.Cqrs.Queries;

public abstract class QueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    public abstract Task HandleAsync(TQuery @event);
}
