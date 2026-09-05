// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.Configuration;

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

#if NET8_0_OR_GREATER
        if (services.Any(service => !service.IsKeyedService && service.ServiceType == typeof(IDccDaprConfigurationProvider)))
            return builder;
#else
        if (services.Any(service => service.ServiceType == typeof(IDccDaprConfigurationProvider)))
            return builder;
#endif

        services.AddSingleton<IDccDaprConfigurationProvider, DccDaprConfigurationProvider>();
        services.AddDaprClient();

        var dccConfigurationOptions = ComplementAndCheckDccConfigurationOption(builder, dccOptions);

        var globalJsonSerializerOptions = MasaApp.GetJsonSerializerOptions();
        var jsonSerializerOption = globalJsonSerializerOptions != null
            ? new JsonSerializerOptions(globalJsonSerializerOptions)
            {
                PropertyNameCaseInsensitive = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            }
            : new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

        jsonSerializerOptions?.Invoke(jsonSerializerOption);

        var callerName = DEFAULT_CLIENT_NAME;
        services.AddCaller(callerName, options =>
        {
            if (action == null)
            {
                options.UseHttpClient(client => client.BaseAddress = dccConfigurationOptions.ManageServiceAddress);
            }
            else
            {
                action.Invoke(options);
            }
        });

        TryAddConfigurationApiClient(
            services,
            dccOptions,
            dccConfigurationOptions.ExpandSections,
            jsonSerializerOption);

        TryAddConfigurationApiManage(
            services,
            callerName,
            dccConfigurationOptions.DefaultSection,
            dccConfigurationOptions.ExpandSections,
            jsonSerializerOption);

        var serviceProvider = services.BuildServiceProvider();
        var configurationApiClient = serviceProvider.GetRequiredService<IConfigurationApiClient>();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        builder.AddRepository(new Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal.DccConfigurationRepository(
            dccConfigurationOptions.GetAllSections(),
            configurationApiClient,
            loggerFactory));
        return builder;
    }

    public static IServiceCollection TryAddConfigurationApiClient(
        IServiceCollection services,
        DccDaprOptions dccOptions,
        List<DccSectionOptions> expansionSectionOptions,
        JsonSerializerOptions jsonSerializerOption)
    {
        services.TryAddSingleton(serviceProvider => DaprDccFactory.CreateClient(
            serviceProvider,
            jsonSerializerOption,
            dccOptions,
            expansionSectionOptions));
        return services;
    }

    public static IServiceCollection TryAddConfigurationApiManage(
        IServiceCollection services,
        string callerName,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions> expansionSectionOptions,
        JsonSerializerOptions jsonSerializerOptions)
    {
        services.TryAddScoped(serviceProvider =>
        {
            var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
            return DaprDccFactory.CreateManage(
                callerFactory.Create(callerName),
                defaultSectionOption,
                jsonSerializerOptions,
                expansionSectionOptions);
        });
        return services;
    }

    public static DccDaprConfigurationOptions ComplementAndCheckDccConfigurationOption(
        IMasaConfigurationBuilder builder,
        DccDaprOptions dccOptions)
    {
        DccDaprConfigurationOptions dccConfigurationOptions = dccOptions;
        CheckDccConfigurationOptions(dccConfigurationOptions);

        var serviceProvider = builder.Services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        MasaAppConfigureOptions? masaAppConfigureOptions = null;
        dccConfigurationOptions.PublicId ??= GetMasaAppConfigureOptions().GetValue(nameof(DccDaprOptions.PublicId), () => DEFAULT_PUBLIC_ID);
        dccConfigurationOptions.PublicSecret ??= GetMasaAppConfigureOptions().GetValue(nameof(DccDaprOptions.PublicSecret));

        var daprClient = scope.ServiceProvider.GetRequiredService<DaprClient>();
        dccConfigurationOptions.DefaultSection.ComplementAndCheckAppId(GetMasaAppConfigureOptions().AppId);
        dccConfigurationOptions.DefaultSection.ComplementAndCheckEnvironment(GetMasaAppConfigureOptions().Environment);
        dccConfigurationOptions.DefaultSection.ComplementAndCheckCluster(GetMasaAppConfigureOptions().Cluster);
        dccConfigurationOptions.DefaultSection.ComplementConfigObjects(daprClient, dccConfigurationOptions.StoreName, DEFAULT_PREFIX);

        if (dccConfigurationOptions.ExpandSections.All(section => section.AppId != dccConfigurationOptions.PublicId))
        {
            dccConfigurationOptions.ExpandSections.Add(new DccSectionOptions
            {
                AppId = dccConfigurationOptions.PublicId!,
                Secret = dccConfigurationOptions.PublicSecret
            });
        }

        DccConfig.AppId = dccConfigurationOptions.DefaultSection.AppId;
        DccConfig.PublicId = dccConfigurationOptions.PublicId!;

        if (dccConfigurationOptions.ExpandSections.Any(sectionOption =>
                sectionOption.AppId == dccConfigurationOptions.DefaultSection.AppId))
        {
            throw new ArgumentException("The extension AppId cannot be the same as the default AppId", nameof(dccOptions));
        }

        foreach (var sectionOption in dccConfigurationOptions.ExpandSections)
        {
            sectionOption.ComplementAndCheckEnvironment(dccConfigurationOptions.DefaultSection.Environment);
            sectionOption.ComplementAndCheckCluster(dccConfigurationOptions.DefaultSection.Cluster);
            sectionOption.ComplementConfigObjects(daprClient, dccConfigurationOptions.StoreName, DEFAULT_PREFIX);
        }

        return dccConfigurationOptions;

        MasaAppConfigureOptions GetMasaAppConfigureOptions()
            => masaAppConfigureOptions ??= scope.ServiceProvider.GetRequiredService<IOptions<MasaAppConfigureOptions>>().Value;
    }

    private static void CheckDccConfigurationOptions(DccDaprConfigurationOptions dccOptions)
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(dccOptions.ManageServiceAddress);
        MasaArgumentException.ThrowIfNullOrWhiteSpace(dccOptions.StoreName);

        dccOptions.ExpandSections.ForEach(section =>
        {
            MasaArgumentException.ThrowIfNullOrWhiteSpace(section.AppId);
        });

        if (dccOptions.ExpandSections.DistinctBy(section => section.AppId).Count() != dccOptions.ExpandSections.Count)
            throw new ArgumentException("AppId cannot be repeated", nameof(dccOptions));
    }

    internal interface IDccDaprConfigurationProvider
    {
    }

    internal sealed class DccDaprConfigurationProvider : IDccDaprConfigurationProvider
    {
    }
}
