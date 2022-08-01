// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Internal;

internal static class DeleteMultiExtensions
{
    public static Task<Nest.BulkResponse> DeleteManyAsync(
        this IElasticClient client,
        IEnumerable<string> documentIds,
        IndexName index,
        CancellationToken cancellationToken = default )
    {
        var bulkRequest = CreateDeleteBulkRequest(documentIds, index);
        return client.BulkAsync(bulkRequest, cancellationToken);
    }

    private static BulkRequest CreateDeleteBulkRequest(IEnumerable<string> documentIds, IndexName index)
    {
        ArgumentNullException.ThrowIfNull(documentIds, nameof(documentIds));
        var bulkRequest = new BulkRequest(index);
        var deletes = documentIds
            .Select(id => new BulkDeleteOperation(new Id(id)))
            .Cast<IBulkOperation>()
            .ToList();

        bulkRequest.Operations = new BulkOperationsCollection<IBulkOperation>(deletes);
        return bulkRequest;
    }
}
