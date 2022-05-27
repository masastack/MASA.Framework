// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Identity;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasaIdentity<TUserId>(this IServiceCollection services)
        where TUserId : IComparable
    {
        services.AddScoped<IUserContext<TUserId>, UserContext<TUserId>>();
        return services;
    }
}
