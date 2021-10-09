using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF.Internal;

internal class QueryFilterProvider : IQueryFilterProvider
{
    public LambdaExpression OnExecuting(IMutableEntityType entityType)
    {
        return default!;
    }
}
