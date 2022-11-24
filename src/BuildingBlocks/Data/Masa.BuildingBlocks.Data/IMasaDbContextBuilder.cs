// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public interface IMasaDbContextBuilder
{
    public bool EnableSoftDelete { get; set; }

    public IServiceCollection Services { get; }
}
