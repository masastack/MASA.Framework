// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Document;

public class SingleDocumentBaseRequest<TDocument> where TDocument : class
{
    public TDocument Document { get; }

    public string? DocumentId { get; }

    public SingleDocumentBaseRequest(TDocument document, string? documentId)
    {
        Document = document;
        DocumentId = documentId ?? Guid.NewGuid().ToString();
    }
}
