// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Document;

public class DocumentOptions
{
    public string IndexName { get; }

    public DocumentOptions(string indexName)
    {
        if (string.IsNullOrEmpty(indexName))
            throw new ArgumentException("indexName cannot be empty",nameof(indexName));

        IndexName = indexName;
    }
}
