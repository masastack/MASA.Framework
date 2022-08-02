// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Document.Exist;

public class ExistDocumentRequest : DocumentOptions
{
    public string DocumentId { get; set; }

    public ExistDocumentRequest(string indexName, string documentId) : base(indexName)
        => DocumentId = documentId;
}
