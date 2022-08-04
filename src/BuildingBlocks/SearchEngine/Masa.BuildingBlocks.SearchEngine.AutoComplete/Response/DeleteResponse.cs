// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete.Response;

public class DeleteResponse : ResponseBase
{
    public DeleteResponse(bool isValid, string message) : base(isValid, message)
    {
    }
}
