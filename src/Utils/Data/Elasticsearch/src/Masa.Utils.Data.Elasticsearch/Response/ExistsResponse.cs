// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Response;

public class ExistsResponse : ResponseBase
{
    public bool Exists { get; }

    public ExistsResponse(Nest.ExistsResponse existsResponse) : base(
        existsResponse.IsValid || existsResponse.ApiCall.HttpStatusCode == 404,
        existsResponse.IsValid || existsResponse.ApiCall.HttpStatusCode == 404 ? "success" : existsResponse.ServerError?.ToString() ?? string.Empty)
    {
        Exists = existsResponse.Exists;
    }
}
