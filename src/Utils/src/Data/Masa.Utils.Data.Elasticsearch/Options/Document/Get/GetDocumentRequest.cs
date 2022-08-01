// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Document.Get;

public class GetDocumentRequest : DocumentOptions
{
    public string Id { get; }

    public GetDocumentRequest(string indexName, string id) : base(indexName) => Id = id;
}
