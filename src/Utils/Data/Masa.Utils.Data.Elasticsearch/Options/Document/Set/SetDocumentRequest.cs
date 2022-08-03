// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Document.Set;

public class SetDocumentRequest<TDocument> : DocumentOptions where TDocument : class
{
    public List<SingleDocumentBaseRequest<TDocument>> Items { get; set; }

    public SetDocumentRequest(string indexName) : base(indexName) => Items = new();

    public SetDocumentRequest(string indexName, TDocument document, string? documentId = null) : this(indexName)
        => AddDocument(document, documentId);

    public SetDocumentRequest(string indexName, IEnumerable<SetDocumentItemBaseRequest<TDocument>> datas) : this(indexName)
    {
        ArgumentNullException.ThrowIfNull(datas, nameof(datas));

        foreach (var data in datas) AddDocument(data.Document, data.DocumentId);
    }

    public SetDocumentRequest<TDocument> AddDocument(TDocument document, string? documentId = null)
        => AddDocument(new SingleDocumentBaseRequest<TDocument>(document, documentId));

    public SetDocumentRequest<TDocument> AddDocument(SingleDocumentBaseRequest<TDocument> item)
    {
        Items.Add(item);
        return this;
    }
}
