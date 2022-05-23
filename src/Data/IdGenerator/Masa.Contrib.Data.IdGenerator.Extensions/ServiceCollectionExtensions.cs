// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Extensions;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection Services;

    public static IServiceCollection AddIdGenerator(this IServiceCollection services, Action<IServiceCollection> options)
    {
        options.Invoke(services);
        Services = services;
        return services;
    }
}
