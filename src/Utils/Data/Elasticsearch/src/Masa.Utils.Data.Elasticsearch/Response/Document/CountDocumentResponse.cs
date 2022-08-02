// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Response.Document;

public class CountDocumentResponse : ResponseBase
{
    public long Count { get; }

    public CountDocumentResponse(CountResponse response) : base(response) => Count = response.Count;
}
