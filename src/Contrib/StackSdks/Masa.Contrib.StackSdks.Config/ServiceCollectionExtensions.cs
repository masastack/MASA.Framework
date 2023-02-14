// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    private static async void InitializeMasaStackConfiguration(this IServiceCollection services)
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

        //services.Configure<MasaStackConfigOptions>(masaStackConfig =>
        //{
        //foreach (var config in configs)
        //{
        //    masaStackConfig.SetValue(config.Key, config.Value);
        //}
        //});

        services.TryAddScoped<IMasaStackConfig>(serviceProvider =>
        {
            var options = new MasaStackConfigOptions();
            foreach (var config in configs)
            {
                options.SetValue(config.Key, config.Value);
            }
            return new MasaStackConfig(options, null);
        });

        var masaStackConfig = services.GetMasaStackConfig();
        var dccOptions = masaStackConfig.GetDefaultDccOptions();
        services.AddMasaConfiguration(builder => builder.UseDcc(dccOptions));

        var serviceProvider2 = services.BuildServiceProvider();
        var configurationApiManage = serviceProvider2.GetRequiredService<IConfigurationApiManage>();
        var configurationApiClient = serviceProvider2.GetRequiredService<IConfigurationApiClient>();

        await configurationApiManage.AddAsync(masaStackConfig.Environment, masaStackConfig.Cluster, DEFAULT_PUBLIC_ID, new Dictionary<string, string>
            {
                { DEFAULT_CONFIG_NAME, JsonSerializer.Serialize(configs) }
            });

        var (Raw, ConfigurationType) = await configurationApiClient.GetRawAsync(masaStackConfig.Environment,
            masaStackConfig.Cluster,
            DEFAULT_PUBLIC_ID,
            DEFAULT_CONFIG_NAME,
            value =>
            {
                UpdateMasaStackConfigContent(masaStackConfig, value);
            });
    }

    public static IServiceCollection AddMasaStackConfig(this IServiceCollection services, bool init = false)
    {
        if (init)
        {
            InitializeMasaStackConfiguration(services);
        }
        else
        {
            services.TryAddScoped<IMasaStackConfig>(serviceProvider =>
            {
                var client = serviceProvider.GetRequiredService<IConfigurationApiClient>();
                return new MasaStackConfig(null, client);
            });
        }

        return services;
    }

    public static IMasaStackConfig GetMasaStackConfig(this IServiceCollection services)
    {
        return services.BuildServiceProvider().GetRequiredService<IMasaStackConfig>();
    }

    private static void UpdateMasaStackConfigContent(IMasaStackConfig masaStackConfig, string content)
    {
        var remoteRaw = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
        if (remoteRaw != null)
        {
            foreach (var config in remoteRaw)
            {
                masaStackConfig.SetValue(config.Key, config.Value);
            }
        }
    }
}
