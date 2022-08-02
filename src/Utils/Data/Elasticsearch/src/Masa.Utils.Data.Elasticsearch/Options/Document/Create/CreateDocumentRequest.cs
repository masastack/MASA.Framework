// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Document.Create;

public class CreateDocumentRequest<TDocument> : DocumentOptions where TDocument : class
{
    public SingleDocumentBaseRequest<TDocument> Request { get; }

    public CreateDocumentRequest(string indexName, TDocument document, string? documentId) : base(indexName)
        => Request = new SingleDocumentBaseRequest<TDocument>(document, documentId);
}
