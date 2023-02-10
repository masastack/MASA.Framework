// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    private static void InitializeMasaStackConfiguration(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        services.Configure<MasaStackConfigOptions>(masaStackConfig =>
        {
            masaStackConfig.SetValue(MasaStackConfigConstant.VERSION, configuration.GetValue<string>(MasaStackConfigConstant.VERSION));
            masaStackConfig.SetValue(MasaStackConfigConstant.IS_DEMO, configuration.GetValue<bool>(MasaStackConfigConstant.IS_DEMO).ToString());
            masaStackConfig.SetValue(MasaStackConfigConstant.DOMAIN_NAME, configuration.GetValue<string>(MasaStackConfigConstant.DOMAIN_NAME));
            masaStackConfig.SetValue(MasaStackConfigConstant.NAMESPACE, configuration.GetValue<string>(MasaStackConfigConstant.NAMESPACE));
            masaStackConfig.SetValue(MasaStackConfigConstant.TLS_NAME, configuration.GetValue<string>(MasaStackConfigConstant.TLS_NAME));
            masaStackConfig.SetValue(MasaStackConfigConstant.CLUSTER, configuration.GetValue<string>(MasaStackConfigConstant.CLUSTER));
            masaStackConfig.SetValue(MasaStackConfigConstant.OTLP_URL, configuration.GetValue<string>(MasaStackConfigConstant.OTLP_URL));
            masaStackConfig.SetValue(MasaStackConfigConstant.REDIS, configuration.GetValue<string>(MasaStackConfigConstant.REDIS));
            masaStackConfig.SetValue(MasaStackConfigConstant.CONNECTIONSTRING, configuration.GetValue<string>(MasaStackConfigConstant.CONNECTIONSTRING));
            masaStackConfig.SetValue(MasaStackConfigConstant.MASA_SERVER, configuration.GetValue<string>(MasaStackConfigConstant.MASA_SERVER));
            masaStackConfig.SetValue(MasaStackConfigConstant.MASA_UI, configuration.GetValue<string>(MasaStackConfigConstant.MASA_UI));
            masaStackConfig.SetValue(MasaStackConfigConstant.ELASTIC, configuration.GetValue<string>(MasaStackConfigConstant.ELASTIC));
            masaStackConfig.SetValue(MasaStackConfigConstant.ENVIRONMENT, configuration.GetValue<string>(MasaStackConfigConstant.ENVIRONMENT));
            masaStackConfig.SetValue(MasaStackConfigConstant.ADMIN_PWD, configuration.GetValue<string>(MasaStackConfigConstant.ADMIN_PWD));
            masaStackConfig.SetValue(MasaStackConfigConstant.DCC_SECRET, configuration.GetValue<string>(MasaStackConfigConstant.DCC_SECRET));
        });

        services.AddMasaConfiguration(builder => builder.UseDcc());
    }

    public static IServiceCollection AddMasaStackConfig(this IServiceCollection services, bool init = true)
    {
        services.TryAddSingleton<IMasaStackConfig, MasaStackConfig>();
        if (init)
        {
            InitializeMasaStackConfiguration(services);
        }
        return services;
    }

    public static IMasaStackConfig GetMasaStackConfig(this IServiceCollection services)
    {
        return services.BuildServiceProvider().GetRequiredService<IMasaStackConfig>();
    }

    //private static DccOptions GetDefaultDccOptions(this IMasaStackConfig masaStackConfig)
    //{
    //    var dccServerAddress = GetDccServiceDomain(masaStackConfig);
    //    var redis = masaStackConfig.RedisModel ?? throw new Exception("redis options can not null");

    //    var stringBuilder = new StringBuilder(@"{""ManageServiceAddress"":");
    //    stringBuilder.Append($"\"{dccServerAddress}\",");
    //    stringBuilder.Append(@"""RedisOptions"": {""Servers"": [{""Host"": ");
    //    stringBuilder.Append($"\"{redis.RedisHost}\",");
    //    stringBuilder.Append(@$"""Port"":{redis.RedisPort}");
    //    stringBuilder.Append("}],");
    //    stringBuilder.Append(@$"""DefaultDatabase"":{redis.RedisDb},");
    //    stringBuilder.Append(@$"""Password"": ""{redis.RedisPassword}""");
    //    stringBuilder.Append(@"},");
    //    stringBuilder.Append(@"""ConfigObjectSecret"":");
    //    stringBuilder.Append($"\"{masaStackConfig.DccSecret}\",");
    //    stringBuilder.Append(@"""PublicSecret"":");
    //    stringBuilder.Append($"\"{masaStackConfig.DccSecret}\"");
    //    stringBuilder.Append('}');

    //    return JsonSerializer.Deserialize<T>(stringBuilder.ToString()) ?? throw new JsonException();
    //}
}
