// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Document.Set;

public class SetDocumentItemBaseRequest<TDocument> : SingleDocumentBaseRequest<TDocument>
    where TDocument : class
{
    public SetDocumentItemBaseRequest(TDocument document, string? documentId) : base(document, documentId)
    {
    }
}
