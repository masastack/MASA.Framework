// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.DependencyInjection.Tests.Services;

public class GoodsService : GoodsBaseService
{
    public static int GoodsCount { get; set; } = 0;

    public GoodsService()
    {
        GoodsCount++;
    }
}
