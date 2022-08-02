// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete.Response;

public class GetResponse<TDropdownBox> : ResponseBase
    where TDropdownBox : AutoCompleteDocument
{
    public long Total { get; set; }

    public long TotalPages { get; set; }

    public List<TDropdownBox> Data { get; set; }

    public GetResponse(bool isValid, string message) : base(isValid, message)
    {
    }

    public GetResponse(bool isValid, string message, IEnumerable<TDropdownBox> data) : this(isValid, message)
    {
        ArgumentNullException.ThrowIfNull(data, nameof(data));

        Data = data.ToList();
    }
}
