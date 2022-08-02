// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Response;

public class GetResponse<TDocument> : ResponseBase
    where TDocument : class
{
    public TDocument Document { get; set; }

    public GetResponse(IGetResponse<TDocument> getResponse) : base(getResponse)
    {
        Document = getResponse.Source;
    }
}
