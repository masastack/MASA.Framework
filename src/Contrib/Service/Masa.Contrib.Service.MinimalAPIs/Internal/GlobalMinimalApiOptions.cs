// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Service.MinimalAPIs.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Service.MinimalAPIs;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal static class GlobalMinimalApiOptions
{
#pragma warning disable S2223
    public static WebApplication? WebApplication;
#pragma warning restore S2223
    public static List<Type> ServiceTypes { get; private set; } = new();

    public static void InitializeService()
    {
        ServiceTypes = new List<Type>();
    }

    public static void AddService(Type serviceType)
    {
        if (ServiceTypes.Contains(serviceType))
            return;

        ServiceTypes.Add(serviceType);
    }
}
