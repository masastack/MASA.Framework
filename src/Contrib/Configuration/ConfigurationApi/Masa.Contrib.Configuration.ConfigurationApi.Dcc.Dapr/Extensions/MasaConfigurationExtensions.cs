// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Dapr;

public static class MasaConfigurationExtensions
{
    public static IMasaConfigurationBuilder UseDcc(
        this IMasaConfigurationBuilder builder,
        Action<JsonSerializerOptions>? jsonSerializerOptions = null,
        Action<CallerBuilder>? callerBuilder = null,
        string sectionName = "DccOptions")
    {
        var configurationSection = builder.Configuration.GetSection(sectionName);
        var dccOptions = configurationSection.Get<DccDaprOptions>();
        MasaArgumentException.ThrowIfNull(dccOptions);
        return builder.UseDcc(dccOptions, jsonSerializerOptions, callerBuilder);
    }

    public static IMasaConfigurationBuilder UseDcc(
        this IMasaConfigurationBuilder builder,
        DccDaprOptions dccOptions,
        Action<JsonSerializerOptions>? jsonSerializerOptions = null,
        Action<CallerBuilder>? action = null)
    {
        var services = builder.Services;

#if (NET8_0_OR_GREATER)
        if (services.Any(service => !service.IsKeyedService && service.ServiceType == typeof(IConfigurationApiClient)))
            return builder;
#else
        if (services.Any(service => service.ServiceType == typeof(IConfigurationApiClient)))
            return builder;
#endif        
        var globalJsonSerializerOptions = MasaApp.GetJsonSerializerOptions();
        var jsonSerializerOption = globalJsonSerializerOptions != null ?
            new JsonSerializerOptions(globalJsonSerializerOptions)
            {
                PropertyNameCaseInsensitive = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            } :
            new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

        jsonSerializerOptions?.Invoke(jsonSerializerOption);
        string callerName = Constants.DEFAULT_CLIENT_NAME;
        services.AddCaller(callerName, options =>
        {
            if (action == null)
            {
                options.UseHttpClient(client => client.BaseAddress = dccOptions.ManageServiceAddress);
            }
            else
            {
                action.Invoke(options);
            }
        });

        TryAddConfigurationApiClient(services,
            dccOptions,
            dccOptions.ExpandSections,
            jsonSerializerOption);

        TryAddConfigurationApiManage(services,
            callerName,
            dccOptions,
            dccOptions.ExpandSections,
            jsonSerializerOption);

        var serviceProvider = services.BuildServiceProvider();

        var configurationApiClient = serviceProvider.GetRequiredService<IConfigurationApiClient>();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        builder.AddRepository(new DccConfigurationRepository(dccOptions.GetAllSections(),
            configurationApiClient, loggerFactory));
        return builder;
    }

    public static IServiceCollection TryAddConfigurationApiClient(IServiceCollection services,
        DccDaprOptions dccOptions,
        List<DccSectionOptions> expansionSectionOptions,
        JsonSerializerOptions jsonSerializerOption)
    {
        services.TryAddSingleton(serviceProvider =>
        {
            return DccFactory.CreateClient(
                serviceProvider,
                jsonSerializerOption,
                dccOptions,
                expansionSectionOptions);
        });
        return services;
    }

    public static IServiceCollection TryAddConfigurationApiManage(IServiceCollection services,
        string callerName,
        DccDaprOptions defaultSectionOption,
        List<DccSectionOptions> expansionSectionOptions,
        JsonSerializerOptions jsonSerializerOptions)
    {
        services.TryAddScoped(serviceProvider =>
        {
            var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
            return DccFactory.CreateManage(callerFactory.Create(callerName), defaultSectionOption, jsonSerializerOptions, expansionSectionOptions);
        });
        return services;
    }
}
