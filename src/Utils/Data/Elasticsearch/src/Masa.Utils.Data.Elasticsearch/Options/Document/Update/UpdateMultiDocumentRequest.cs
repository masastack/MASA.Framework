// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Document.Update;

public class UpdateMultiDocumentRequest<TDocument> : DocumentOptions where TDocument : class
{
    public List<UpdateDocumentBaseRequest<TDocument>> Items { get; set; }

    public UpdateMultiDocumentRequest(string indexName) : base(indexName)
        => Items = new();

    public UpdateMultiDocumentRequest<TDocument> AddDocument(UpdateDocumentBaseRequest<TDocument> item)
    {
        Items.Add(item);
        return this;
    }
}
