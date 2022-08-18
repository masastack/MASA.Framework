// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Document.Query;

public class QueryBaseOptions<TDocument> : DocumentOptions
    where TDocument : class
{
    public string? DefaultField { get; }

    public IEnumerable<string> Fields { get; private set; }

    public string Query { get; }

    public string Analyzer { get; set; }

    public Operator Operator { get; set; }

    public Action<QueryStringQueryDescriptor<TDocument>>? Action { get; set; }

    public QueryBaseOptions(string indexName, string query, string? defaultField = null, Operator @operator = Operator.Or)
        : base(indexName)
    {
        DefaultField = defaultField;
        Fields = Array.Empty<string>();
        Query = query;
        Operator = @operator;
    }

    public QueryBaseOptions(string indexName, string query, IEnumerable<string>? fields = null, Operator @operator = Operator.Or)
        : base(indexName)
    {
        DefaultField = string.Empty;
        Fields = fields ?? Array.Empty<string>();
        Query = query;
        Operator = @operator;
    }

    public QueryBaseOptions<TDocument> UseFields(params string[] fields)
    {
        if (string.IsNullOrEmpty(DefaultField))
            throw new NotSupportedException("Does not support the assignment of DefaultField and Fields at the same time");

        Fields = fields;
        return this;
    }
}
