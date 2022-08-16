// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLadpContext(this IServiceCollection services, Action<LdapOptions> optionsAction)
    {
        services.Configure(optionsAction);
        services.AddSingleton(typeof(ILdapProvider), typeof(LdapProvider));
        return services;
    }

    public static IServiceCollection AddLadpContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LdapOptions>(configuration);
        services.AddSingleton(typeof(ILdapProvider), typeof(LdapProvider));
        return services;
    }

    public static IServiceCollection AddLadpContext(this IServiceCollection services)
    {
        services.AddSingleton(typeof(ILdapFactory), typeof(LdapFactory));
        return services;
    }
}
