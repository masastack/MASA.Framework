// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete.Response;

public class DeleteMultiResponse : ResponseBase
{
    public List<DeleteRangeResponseItems> Data { get; set; }

    public DeleteMultiResponse(bool isValid, string message) : base(isValid, message)
    {
    }

    public DeleteMultiResponse(bool isValid, string message, IEnumerable<DeleteRangeResponseItems>? data) : this(isValid, message)
    {
        ArgumentNullException.ThrowIfNull(data, nameof(data));

        Data = data.ToList();
    }
}
