// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options.Document.Get;

public class GetMultiDocumentRequest : DocumentOptions
{
    public IEnumerable<string> Ids { get; }

    public GetMultiDocumentRequest(string indexName,params string[] ids) : base(indexName)
        => Ids = ids;

    public GetMultiDocumentRequest(string indexName, IEnumerable<string> ids) : base(indexName)
        => Ids = ids;
}
