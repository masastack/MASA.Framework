// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdGeneratorCore(this IServiceCollection services)
    {
        services.AddServiceFactory();
        services.AddSingleton<IIdGeneratorFactory, DefaultIdGeneratorFactory>();
        return services;
    }
}
