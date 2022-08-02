// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Alias;

public class UnBindAliasIndexOptions
{
    public IEnumerable<string> IndexNames { get; } = default!;

    public string Alias { get; }

    private UnBindAliasIndexOptions(string alias) => Alias = alias;

    public UnBindAliasIndexOptions(string alias, string indexName) : this(alias)
    {
        if (string.IsNullOrEmpty(indexName))
            throw new ArgumentException("indexName cannot be empty", nameof(indexName));

        IndexNames = new[] { indexName };
    }

    public UnBindAliasIndexOptions(string alias, IEnumerable<string> indexNames) : this(alias)
    {
        ArgumentNullException.ThrowIfNull(nameof(indexNames));
        IndexNames = indexNames;
    }
}
