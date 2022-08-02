// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Document.Create;

public class CreateMultiDocumentRequest<TDocument> : DocumentOptions where TDocument : class
{
    public List<SingleDocumentBaseRequest<TDocument>> Items { get; set; }

    public CreateMultiDocumentRequest(string indexName) : base(indexName) => Items = new();

    public CreateMultiDocumentRequest(string indexName, TDocument document, string? documentId = null) : this(indexName)
        => AddDocument(document, documentId);

    public CreateMultiDocumentRequest(string indexName, IEnumerable<CreateDocumentItemRequest<TDocument>> datas) : this(indexName)
    {
        ArgumentNullException.ThrowIfNull(datas, nameof(datas));

        foreach (var data in datas) AddDocument(data.Document, data.DocumentId);
    }

    public CreateMultiDocumentRequest<TDocument> AddDocument(TDocument document, string? documentId = null)
        => AddDocument(new SingleDocumentBaseRequest<TDocument>(document, documentId));

    public CreateMultiDocumentRequest<TDocument> AddDocument(SingleDocumentBaseRequest<TDocument> item)
    {
        Items.Add(item);
        return this;
    }
}
