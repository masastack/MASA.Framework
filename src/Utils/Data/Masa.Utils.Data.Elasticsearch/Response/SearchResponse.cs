// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Response;

public class SearchResponse<TDocument> : ResponseBase
    where TDocument : class
{
    public List<TDocument> Data { get; }

    public SearchResponse(ISearchResponse<TDocument> searchResponse) : base(searchResponse)
    {
        Data = searchResponse.Hits.Select(hit => hit.Source).ToList();
    }
}
