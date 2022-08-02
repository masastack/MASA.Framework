// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Document.Query;

public class PaginatedOptions<TDocument> : QueryBaseOptions<TDocument>
    where TDocument : class
{
    public int Page { get; }

    public int PageSize { get; }

    public PaginatedOptions(
        string indexName,
        string query,
        string defaultField,
        int page,
        int pageSize,
        Operator @operator = Operator.Or)
        : base(indexName, query, defaultField, @operator)
    {
        Page = page;
        PageSize = pageSize;
    }

    public PaginatedOptions(
        string indexName,
        string query,
        IEnumerable<string> fields,
        int page,
        int pageSize,
        Operator @operator = Operator.Or)
        : base(indexName, query, fields, @operator)
    {
        Page = page;
        PageSize = pageSize;
    }

    public new PaginatedOptions<TDocument> UseFields(params string[] fields)
    {
        base.UseFields(fields);
        return this;
    }
}
