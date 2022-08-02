// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete.Response;

public class SetResponseItems
{
    public string Id { get; }

    public bool IsValid { get; }

    public string Message { get; }

    public SetResponseItems(string id, bool isValid, string message)
    {
        Id = id;
        IsValid = isValid;
        Message = message;
    }
}
