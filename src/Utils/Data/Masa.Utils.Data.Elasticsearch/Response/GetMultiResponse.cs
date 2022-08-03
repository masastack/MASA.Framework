// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Response;

public class GetMultiResponse<TDocument> : ResponseBase
    where TDocument : class
{
    public List<GetMultiResponseItems<TDocument>> Data { get; set; }

    public GetMultiResponse(bool isValid, string message, List<IMultiGetHit<TDocument>>? multiGetHits = null) : base(isValid, message)
    {
        Data = multiGetHits?.Select(res => new GetMultiResponseItems<TDocument>(res.Id, res.Source)).ToList() ?? new();
    }
}
