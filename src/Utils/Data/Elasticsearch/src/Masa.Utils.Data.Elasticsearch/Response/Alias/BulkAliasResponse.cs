// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Response.Alias;

public class BulkAliasResponse : ResponseBase
{
    public BulkAliasResponse(Nest.BulkAliasResponse bulkAliasResponse) : base(bulkAliasResponse)
    {
    }
}
