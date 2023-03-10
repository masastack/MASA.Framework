﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.BuildingBlocks.Service.Caller")]

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

internal static class ServiceCollectionExtensions
{
    public static void AddServiceFactory(this IServiceCollection services)
    {
        services.TryAddSingleton<ServiceSingleton>();
        services.TryAddScoped<ServiceScoped>();
    }
}
