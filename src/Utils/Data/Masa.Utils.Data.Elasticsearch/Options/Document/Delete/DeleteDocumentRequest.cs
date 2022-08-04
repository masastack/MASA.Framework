// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Document.Delete;

public class DeleteDocumentRequest : DocumentOptions
{
    public string DocumentId { get; }

    public DeleteDocumentRequest(string indexName, string documentId) : base(indexName) => DocumentId = documentId;
}
