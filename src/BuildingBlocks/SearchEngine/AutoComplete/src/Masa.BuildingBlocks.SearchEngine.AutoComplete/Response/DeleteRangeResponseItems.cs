// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete.Response;

public class DeleteRangeResponseItems
{
    public string Id { get; }

    public bool IsValid { get; }

    public string Message { get; }

    public DeleteRangeResponseItems(string id, bool isValid, string message)
    {
        this.Id = id;
        this.IsValid = isValid;
        this.Message = message;
    }
}
