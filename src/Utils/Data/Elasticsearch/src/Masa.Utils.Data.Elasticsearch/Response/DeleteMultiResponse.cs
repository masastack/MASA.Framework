// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Response;

public class DeleteMultiResponse : ResponseBase
{
    public List<DeleteRangeResponseItems> Data { get; set; }

    public DeleteMultiResponse(Nest.BulkResponse bulkResponse) : base(bulkResponse)
    {
        Data = bulkResponse.Items.Select(item =>
            new DeleteRangeResponseItems(item.Id,
                item.IsValid && item.Status == 200,
                !string.IsNullOrEmpty(item.Result) ? item.Result : item.Error?.ToString() ?? string.Empty)).ToList();
    }
}
