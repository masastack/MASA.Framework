// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Response;

public class GetMultiResponseItems<TDocument>
    where TDocument : class
{
    public string Id { get; }

    public TDocument Document { get; }

    public GetMultiResponseItems(string id, TDocument document)
    {
        Id = id;
        Document = document;
    }
}
