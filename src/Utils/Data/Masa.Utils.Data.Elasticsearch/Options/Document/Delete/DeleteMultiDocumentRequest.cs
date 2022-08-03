// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Document.Delete;

public class DeleteMultiDocumentRequest : DocumentOptions
{
    public IEnumerable<string> DocumentIds { get; }

    public DeleteMultiDocumentRequest(string indexName, params string[] documentIds) : base(indexName)
        => DocumentIds = documentIds;

    public DeleteMultiDocumentRequest(string indexName, IEnumerable<string> documentIds) : base(indexName)
        => DocumentIds = documentIds;
}
