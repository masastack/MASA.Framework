// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Response.Index;

public class GetIndexResponse : ResponseBase
{
    public string[] IndexNames { get; set; }

    public GetIndexResponse(CatResponse<CatIndicesRecord> catResponse) : base(catResponse)
    {
        IndexNames = catResponse.IsValid ? catResponse.Records.Select(r => r.Index).ToArray() : Array.Empty<string>();
    }
}
