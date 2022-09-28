﻿// Copyright (c) MASA Stack All rights reserved.
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

    private static Assembly[]? Assemblies { get; set; }

    /// <summary>
    /// Global JsonSerializerOptions configuration
    /// </summary>
    public static JsonSerializerOptions? JsonSerializerOptions { get; set; }

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
        if (Services.Count == 0)
        {
            Services = services;
            _rootServiceProvider = null;
        }
    }

    /// <summary>
    /// Set the global Assembly collection (only if Assembly is not assigned a value)
    /// </summary>
    /// <param name="assemblies"></param>
    public static void TrySetAssemblies(params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);

        Assemblies ??= assemblies;
    }

    /// <summary>
    /// Set the global Assembly collection
    /// </summary>
    /// <param name="assemblies"></param>
    public static void SetAssemblies(params Assembly[] assemblies)
        => Assemblies = assemblies;

    public static Assembly[] GetAssemblies() => Assemblies ?? AppDomain.CurrentDomain.GetAssemblies();
}
