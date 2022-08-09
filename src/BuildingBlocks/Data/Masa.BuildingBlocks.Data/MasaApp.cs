// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class MasaApp
{
    private static bool _initialized;
    public static IServiceProvider RootServices { get; private set; }

    public static void TrySetRootServices(IServiceProvider serviceProvider, bool checkInitialize = true)
    {
        if (!checkInitialize)
            RootServices = serviceProvider;
        else if (!_initialized)
        {
            _initialized = true;
            RootServices = serviceProvider;
        }
    }

    public static TService? GetService<TService>()
        => GetService<TService>(RootServices);

    public static TService? GetService<TService>(IServiceProvider serviceProvider)
        => serviceProvider.GetService<TService>();

    public static TService GetRequiredService<TService>() where TService : notnull
        => GetRequiredService<TService>(RootServices);

    public static TService GetRequiredService<TService>(IServiceProvider serviceProvider) where TService : notnull
        => serviceProvider.GetRequiredService<TService>();
}
