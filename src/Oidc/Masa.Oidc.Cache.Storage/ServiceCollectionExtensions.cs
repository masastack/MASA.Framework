// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.Cache.Storage;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOidcCache(this IServiceCollection services)
    {
        services.AddSingleton<IClientStore, ClientStore>();
        services.AddSingleton<IResourceStore, ResourceStore>();

        return services;
    }
}
