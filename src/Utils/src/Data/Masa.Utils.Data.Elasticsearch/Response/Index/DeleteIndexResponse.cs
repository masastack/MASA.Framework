// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Response.Index;

public class DeleteIndexResponse : ResponseBase
{
    public DeleteIndexResponse(Nest.DeleteIndexResponse deleteIndexResponse) : base(deleteIndexResponse)
    {
    }

    public DeleteIndexResponse(BulkAliasResponse bulkAliasResponse) : base(bulkAliasResponse)
    {
    }

    public DeleteIndexResponse(string message) : base(false, message)
    {
    }
}
