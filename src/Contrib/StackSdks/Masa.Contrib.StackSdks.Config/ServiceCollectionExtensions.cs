// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    private static async Task InitializeMasaStackConfiguration(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        var configs = new Dictionary<string, string>()
        {
            { MasaStackConfigConstant.VERSION, configuration.GetValue<string>(MasaStackConfigConstant.VERSION) },
            { MasaStackConfigConstant.IS_DEMO, configuration.GetValue<bool>(MasaStackConfigConstant.IS_DEMO).ToString() },
            { MasaStackConfigConstant.DOMAIN_NAME, configuration.GetValue<string>(MasaStackConfigConstant.DOMAIN_NAME) },
            { MasaStackConfigConstant.NAMESPACE, configuration.GetValue<string>(MasaStackConfigConstant.NAMESPACE) },
            { MasaStackConfigConstant.TLS_NAME, configuration.GetValue<string>(MasaStackConfigConstant.TLS_NAME) },
            { MasaStackConfigConstant.CLUSTER, configuration.GetValue<string>(MasaStackConfigConstant.CLUSTER) },
            { MasaStackConfigConstant.OTLP_URL, configuration.GetValue<string>(MasaStackConfigConstant.OTLP_URL) },
            { MasaStackConfigConstant.REDIS, configuration.GetValue<string>(MasaStackConfigConstant.REDIS) },
            { MasaStackConfigConstant.CONNECTIONSTRING, configuration.GetValue<string>(MasaStackConfigConstant.CONNECTIONSTRING) },
            { MasaStackConfigConstant.MASA_SERVER, configuration.GetValue<string>(MasaStackConfigConstant.MASA_SERVER) },
            { MasaStackConfigConstant.MASA_UI, configuration.GetValue<string>(MasaStackConfigConstant.MASA_UI) },
            { MasaStackConfigConstant.ELASTIC, configuration.GetValue<string>(MasaStackConfigConstant.ELASTIC) },
            { MasaStackConfigConstant.ENVIRONMENT, configuration.GetValue<string>(MasaStackConfigConstant.ENVIRONMENT) },
            { MasaStackConfigConstant.ADMIN_PWD, configuration.GetValue<string>(MasaStackConfigConstant.ADMIN_PWD) },
            { MasaStackConfigConstant.DCC_SECRET, configuration.GetValue<string>(MasaStackConfigConstant.DCC_SECRET) }
        };
        services.TryAddSingleton<IMasaStackConfig>(serviceProvider =>
        {
            return new MasaStackConfig(configs, null);
        });

        var masaStackConfig = services.GetMasaStackConfig();
        var dccOptions = masaStackConfig.GetDefaultDccOptions();
        services.AddMasaConfiguration(builder => builder.UseDcc(dccOptions));

        var serviceProvider2 = services.BuildServiceProvider();
        var configurationApiManage = serviceProvider2.GetRequiredService<IConfigurationApiManage>();
        var configurationApiClient = serviceProvider2.GetRequiredService<IConfigurationApiClient>();

        try
        {
            var remoteConfigs = await configurationApiClient.GetAsync<Dictionary<string, string>>(
                masaStackConfig.Environment,
                masaStackConfig.Cluster,
                DEFAULT_PUBLIC_ID,
                DEFAULT_CONFIG_NAME,
                value =>
                {
                    masaStackConfig.UpdateMasaStackConfigContent(value);
                });

            if (remoteConfigs != null)
            {
                await configurationApiManage.UpdateAsync(
                    masaStackConfig.Environment,
                    masaStackConfig.Cluster,
                    DEFAULT_PUBLIC_ID,
                    DEFAULT_CONFIG_NAME,
                    configs);
            }
        }
        catch (ArgumentException)
        {
            await configurationApiManage.AddAsync(masaStackConfig.Environment, masaStackConfig.Cluster, DEFAULT_PUBLIC_ID, new Dictionary<string, string>
            {
                { DEFAULT_CONFIG_NAME, JsonSerializer.Serialize(configs) }
            });
        }
    }

    public static IServiceCollection AddMasaStackConfig(this IServiceCollection services, bool init = false)
    {
        if (init)
        {
            InitializeMasaStackConfiguration(services).ConfigureAwait(false);
        }
        else
        {
            services.TryAddScoped<IMasaStackConfig>(serviceProvider =>
            {
                var client = serviceProvider.GetRequiredService<IConfigurationApiClient>();
                return new MasaStackConfig(new(), client);
            });
        }

        return services;
    }

    public static IMasaStackConfig GetMasaStackConfig(this IServiceCollection services)
    {
        return services.BuildServiceProvider().GetRequiredService<IMasaStackConfig>();
    }

    private static void UpdateMasaStackConfigContent(this IMasaStackConfig masaStackConfig, Dictionary<string, string> configMap)
    {
        if (configMap != null)
        {
            masaStackConfig.SetValues(configMap);
        }
    }
}
