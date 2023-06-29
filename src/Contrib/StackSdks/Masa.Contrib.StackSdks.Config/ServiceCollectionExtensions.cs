// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    private static async Task InitializeMasaStackConfiguration(
        IServiceCollection services,
        Dictionary<string, string> configs)
    {
        var serviceProvider = services.BuildServiceProvider();
        var configurationApiManage = serviceProvider.GetRequiredService<IConfigurationApiManage>();
        var configurationApiClient = serviceProvider.GetRequiredService<IConfigurationApiClient>();

        try
        {
            var remoteConfigs = await configurationApiClient.GetAsync<Dictionary<string, string>>(
                configs[MasaStackConfigConstant.ENVIRONMENT],
                configs[MasaStackConfigConstant.CLUSTER],
                DEFAULT_PUBLIC_ID,
                DEFAULT_CONFIG_NAME);

            if (remoteConfigs != null)
            {
                await configurationApiManage.UpdateAsync(
                    configs[MasaStackConfigConstant.ENVIRONMENT],
                    configs[MasaStackConfigConstant.CLUSTER],
                    DEFAULT_PUBLIC_ID,
                    DEFAULT_CONFIG_NAME,
                    configs);
            }
        }
        catch (ArgumentException)
        {
            // remoteConfigs is null
            await configurationApiManage.AddAsync(
                configs[MasaStackConfigConstant.ENVIRONMENT],
                configs[MasaStackConfigConstant.CLUSTER],
                DEFAULT_PUBLIC_ID,
                new Dictionary<string, object>
                {
                    { DEFAULT_CONFIG_NAME, configs }
                });
        }
    }

    public static async Task<IServiceCollection> AddMasaStackConfigAsync(this IServiceCollection services, MasaStackProject project, MasaStackApp app, bool init = false, DccOptions? dccOptions = null)
    {
        var configs = GetConfigMap(services);

        dccOptions ??= MasaStackConfigUtils.GetDefaultDccOptions(configs, project, app);
        services.AddSingleton(dccOptions);
        services.AddMasaConfiguration(builder => builder.UseDcc(dccOptions));

        if (init)
        {
            await InitializeMasaStackConfiguration(services, configs).ConfigureAwait(false);
        }

        services.TryAddScoped<IMasaStackConfig>(serviceProvider =>
        {
            var configurationApiClient = serviceProvider.GetRequiredService<IConfigurationApiClient>();
            return new MasaStackConfig(configurationApiClient, configs);
        });

        services.TryAddScoped<IMultiEnvironmentMasaStackConfig>(serviceProvider =>
        {
            var configurationApiClient = serviceProvider.GetRequiredService<IConfigurationApiClient>();
            return new MultiEnvironmentMasaStackConfig(configurationApiClient, configs);
        });

        return services;
    }

    public static IMasaStackConfig GetMasaStackConfig(this IServiceCollection services)
    {
        return services.BuildServiceProvider().GetRequiredService<IMasaStackConfig>();
    }

    public static DccOptions? GetDccOptions(this IServiceCollection services)
    {
        return services.BuildServiceProvider().GetService<DccOptions>();
    }

    public static IMultiEnvironmentMasaStackConfig GetMultiEnvironmentMasaStackConfig(this IServiceCollection services)
    {
        return services.BuildServiceProvider().GetRequiredService<IMultiEnvironmentMasaStackConfig>();
    }

    private static Dictionary<string, string> GetConfigMap(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        string environment = configuration.GetValue<string>(MasaStackConfigConstant.ENVIRONMENT);
        environment = string.IsNullOrWhiteSpace(environment) ? configuration["ASPNETCORE_ENVIRONMENT"] : environment;

        var configs = new Dictionary<string, string>()
        {
            { MasaStackConfigConstant.VERSION, configuration.GetValue<string>(MasaStackConfigConstant.VERSION) },
            { MasaStackConfigConstant.IS_DEMO, configuration.GetValue<bool>(MasaStackConfigConstant.IS_DEMO).ToString() },
            { MasaStackConfigConstant.DOMAIN_NAME, configuration.GetValue<string>(MasaStackConfigConstant.DOMAIN_NAME) },
            { MasaStackConfigConstant.NAMESPACE, configuration.GetValue<string>(MasaStackConfigConstant.NAMESPACE) },
            { MasaStackConfigConstant.CLUSTER, configuration.GetValue<string>(MasaStackConfigConstant.CLUSTER) },
            { MasaStackConfigConstant.OTLP_URL, configuration.GetValue<string>(MasaStackConfigConstant.OTLP_URL) },
            { MasaStackConfigConstant.REDIS, configuration.GetValue<string>(MasaStackConfigConstant.REDIS) },
            { MasaStackConfigConstant.CONNECTIONSTRING, configuration.GetValue<string>(MasaStackConfigConstant.CONNECTIONSTRING) },
            { MasaStackConfigConstant.MASA_STACK, configuration.GetValue<string>(MasaStackConfigConstant.MASA_STACK) },
            { MasaStackConfigConstant.ELASTIC, configuration.GetValue<string>(MasaStackConfigConstant.ELASTIC) },
            { MasaStackConfigConstant.ENVIRONMENT, environment },
            { MasaStackConfigConstant.ADMIN_PWD, configuration.GetValue<string>(MasaStackConfigConstant.ADMIN_PWD) },
            { MasaStackConfigConstant.DCC_SECRET, configuration.GetValue<string>(MasaStackConfigConstant.DCC_SECRET) },
            { MasaStackConfigConstant.SUFFIX_IDENTITY, configuration.GetValue<string>(MasaStackConfigConstant.SUFFIX_IDENTITY) }
        };

        return configs;
    }
}
