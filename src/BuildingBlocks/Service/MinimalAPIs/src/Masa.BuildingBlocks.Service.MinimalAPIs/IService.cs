// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.MinimalAPIs;

public interface IService
{
    WebApplication App { get; }

    IServiceCollection Services { get; }

    TService? GetService<TService>();

    TService GetRequiredService<TService>() where TService : notnull;
}
