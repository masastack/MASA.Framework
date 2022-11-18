// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.Contracts.Request;

public class RequestPageBase
{
    public int Page { get; set; } = 1;

    public int Size { get; set; } = 20;
}
