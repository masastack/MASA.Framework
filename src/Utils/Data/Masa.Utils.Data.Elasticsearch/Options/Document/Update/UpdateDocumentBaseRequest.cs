// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Document.Update;

public class UpdateDocumentBaseRequest<TDocument> where TDocument : class
{
    public TDocument? Document { get; }

    public object? PartialDocument { get; }

    public string? DocumentId { get; }

    public UpdateDocumentBaseRequest(TDocument document, string? documentId = null)
    {
        Document = document;
        DocumentId = documentId;
    }

    public UpdateDocumentBaseRequest(object partialDocument, string? documentId = null)
    {
        PartialDocument = partialDocument;
        DocumentId = documentId;
    }
}
