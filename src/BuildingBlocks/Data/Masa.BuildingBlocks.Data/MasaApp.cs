// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public static class MasaApp
{
    private static IServiceProvider? _rootServiceProvider;

    public static IServiceProvider RootServiceProvider
    {
        get
        {
            if (_rootServiceProvider == null) Build();
            return _rootServiceProvider!;
        }
        private set => _rootServiceProvider = value;
    }

    public static IServiceCollection Services { get; set; } = new ServiceCollection();

    public static void Build() => Build(Services.BuildServiceProvider());

    public static void Build(IServiceProvider serviceProvider) => RootServiceProvider = serviceProvider;

    public static TService? GetService<TService>()
        => GetService<TService>(RootServiceProvider);

    public static TService? GetService<TService>(IServiceProvider serviceProvider)
        => serviceProvider.GetService<TService>();

    public static TService GetRequiredService<TService>() where TService : notnull
        => GetRequiredService<TService>(RootServiceProvider);

    public static TService GetRequiredService<TService>(IServiceProvider serviceProvider) where TService : notnull
        => serviceProvider.GetRequiredService<TService>();

    public static void TrySetServiceCollection(IServiceCollection services)
    {
        if (Services.Count == 0) Services = services;
    }
}
