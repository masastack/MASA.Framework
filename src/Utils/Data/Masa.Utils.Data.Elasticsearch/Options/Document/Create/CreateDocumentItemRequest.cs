// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Document.Create;

public class CreateDocumentItemRequest<TDocument> : SingleDocumentBaseRequest<TDocument>
    where TDocument : class
{
    public CreateDocumentItemRequest(TDocument document, string? documentId) : base(document, documentId)
    {
    }
}
