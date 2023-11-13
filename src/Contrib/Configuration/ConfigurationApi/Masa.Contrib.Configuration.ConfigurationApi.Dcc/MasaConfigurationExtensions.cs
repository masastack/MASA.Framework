// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

using YamlDotNet.Core.Tokens;

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
        var dccOptions = configurationSection.Get<DccOptions>();
        return builder.UseDcc(dccOptions, jsonSerializerOptions, callerBuilder);
    }

    public static IMasaConfigurationBuilder UseDcc(
        this IMasaConfigurationBuilder builder,
        DccOptions dccOptions,
        Action<JsonSerializerOptions>? jsonSerializerOptions = null,
        Action<CallerBuilder>? action = null)
    {
        if (string.IsNullOrEmpty(dccOptions.ManageServiceAddress))
            throw new ArgumentException($"ManageServiceAddress cannot be null or empty");

        var services = builder.Services;
        if (services.Any(service => service.ImplementationType == typeof(DccConfigurationProvider)))
            return builder;

        services.AddSingleton<DccConfigurationProvider>();

        var dccConfigurationOptions = ComplementAndCheckDccConfigurationOption(builder, dccOptions);

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
        string callerName = DEFAULT_CLIENT_NAME;
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

        TryAddConfigurationApiClient(services,
            dccOptions,
            dccConfigurationOptions.DefaultSection,
            dccConfigurationOptions.ExpandSections,
            jsonSerializerOption);

        TryAddConfigurationApiManage(services,
            callerName,
            dccConfigurationOptions.DefaultSection,
            dccConfigurationOptions.ExpandSections,
            jsonSerializerOption);

        var serviceProvider = services.BuildServiceProvider();

        var configurationApiClient = serviceProvider.GetRequiredService<IConfigurationApiClient>();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        builder.AddRepository(new DccConfigurationRepository(dccConfigurationOptions.GetAllSections(),
            configurationApiClient, loggerFactory));
        return builder;
    }

    public static IServiceCollection TryAddConfigurationApiClient(IServiceCollection services,
        DccOptions dccOptions,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions> expansionSectionOptions,
        JsonSerializerOptions jsonSerializerOption)
    {
        services.TryAddSingleton(serviceProvider =>
        {
            var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
            return DccFactory.CreateClient(
                serviceProvider,
                jsonSerializerOption,
                dccOptions,
                defaultSectionOption,
                expansionSectionOptions);
        });
        return services;
    }

    public static IServiceCollection TryAddConfigurationApiManage(IServiceCollection services,
        string callerName,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions> expansionSectionOptions,
        JsonSerializerOptions jsonSerializerOptions)
    {
        services.TryAddSingleton(serviceProvider =>
        {
            var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
            return DccFactory.CreateManage(callerFactory.Create(callerName), defaultSectionOption, jsonSerializerOptions, expansionSectionOptions);
        });
        return services;
    }

    public static DccConfigurationOptions ComplementAndCheckDccConfigurationOption(
        IMasaConfigurationBuilder builder,
        DccOptions dccOptions)
    {
        DccConfigurationOptions dccConfigurationOptions = dccOptions;
        CheckDccConfigurationOptions(dccConfigurationOptions);

        var serviceProvider = builder.Services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();

        MasaAppConfigureOptions? masaAppConfigureOptions = null;
        dccConfigurationOptions.PublicId ??= GetMasaAppConfigureOptions().GetValue(nameof(DccOptions.PublicId), () => DEFAULT_PUBLIC_ID);
        dccConfigurationOptions.PublicSecret ??= GetMasaAppConfigureOptions().GetValue(nameof(DccOptions.PublicSecret));

        dccConfigurationOptions.DefaultSection.ComplementAndCheckAppId(GetMasaAppConfigureOptions().AppId);
        dccConfigurationOptions.DefaultSection.ComplementAndCheckEnvironment(GetMasaAppConfigureOptions().Environment);
        dccConfigurationOptions.DefaultSection.ComplementAndCheckCluster(GetMasaAppConfigureOptions().Cluster);
        //dccConfigurationOptions.DefaultSection.ComplementConfigObjects(distributedCacheClient);

        if (dccConfigurationOptions.ExpandSections.All(section => section.AppId != dccConfigurationOptions.PublicId))
        {
            var publicSection = new DccSectionOptions
            {
                AppId = dccConfigurationOptions.PublicId,
                Secret = dccConfigurationOptions.PublicSecret
            };
            dccConfigurationOptions.ExpandSections.Add(publicSection);
        }

        DccConfig.AppId = dccConfigurationOptions.DefaultSection.AppId;
        DccConfig.PublicId = dccConfigurationOptions.PublicId;

        if (dccConfigurationOptions.ExpandSections.Any(sectionOption
                => sectionOption.AppId == dccConfigurationOptions.DefaultSection.AppId))
            throw new ArgumentException("The extension AppId cannot be the same as the default AppId", nameof(dccOptions));

        foreach (var sectionOption in dccConfigurationOptions.ExpandSections)
        {
            sectionOption.ComplementAndCheckEnvironment(dccConfigurationOptions.DefaultSection.Environment);
            sectionOption.ComplementAndCheckCluster(dccConfigurationOptions.DefaultSection.Cluster);
            //sectionOption.ComplementConfigObjects(distributedCacheClient);
        }
        return dccConfigurationOptions;

        MasaAppConfigureOptions GetMasaAppConfigureOptions()
        {
            return masaAppConfigureOptions ??= scope.ServiceProvider.GetRequiredService<IOptions<MasaAppConfigureOptions>>().Value;
        }
    }

    private static void CheckDccConfigurationOptions(DccConfigurationOptions dccOptions)
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(dccOptions.ManageServiceAddress);

        dccOptions.ExpandSections.ForEach(section =>
        {
            MasaArgumentException.ThrowIfNullOrWhiteSpace(section.AppId);
        });

        if (dccOptions.ExpandSections.DistinctBy(dccSectionOptions => dccSectionOptions.AppId).Count() != dccOptions.ExpandSections.Count)
            throw new ArgumentException("AppId cannot be repeated", nameof(dccOptions));
    }

    private sealed class DccConfigurationProvider
    {

    }
}
