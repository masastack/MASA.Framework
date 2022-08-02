// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Alias;

public class BindAliasIndexOptions
{
    public IEnumerable<string> IndexNames { get; } = default!;

    public string Alias { get; }

    private BindAliasIndexOptions(string alias) => Alias = alias;

    public BindAliasIndexOptions(string alias, string indexName) : this(alias)
    {
        if (string.IsNullOrEmpty(indexName))
            throw new ArgumentException("indexName cannot be empty", nameof(indexName));

        IndexNames = new[] { indexName };
    }

    public BindAliasIndexOptions(string alias, IEnumerable<string> indexNames) : this(alias)
    {
        ArgumentNullException.ThrowIfNull(nameof(indexNames));
        IndexNames = indexNames;
    }
}
