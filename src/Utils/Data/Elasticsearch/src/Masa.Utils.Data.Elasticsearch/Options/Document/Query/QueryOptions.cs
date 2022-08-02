// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Document.Query;

public class QueryOptions<TDocument> : QueryBaseOptions<TDocument>
    where TDocument : class
{
    public int Skip { get; }

    public int Take { get; }

    public QueryOptions(string indexName, string query, string defaultField, int skip, int take, Operator @operator = Operator.Or)
        : base(indexName, query, defaultField, @operator)
    {
        Skip = skip;
        Take = take;
    }
}
