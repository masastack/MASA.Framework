// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Data;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdGeneratorCore(this IServiceCollection services)
    {
        services.AddSingleton<IIdGeneratorFactory, DefaultIdGeneratorFactory>();
        return services;
    }
}
