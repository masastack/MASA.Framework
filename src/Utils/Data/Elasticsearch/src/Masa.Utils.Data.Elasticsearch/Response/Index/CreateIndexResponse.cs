// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Response.Index;

public class CreateIndexResponse : ResponseBase
{
    public CreateIndexResponse(Nest.CreateIndexResponse createIndexResponse) : base(createIndexResponse)
    {
    }
}
