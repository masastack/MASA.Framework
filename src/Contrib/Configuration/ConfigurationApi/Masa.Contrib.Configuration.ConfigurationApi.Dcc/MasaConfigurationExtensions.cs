// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;

public static class MasaConfigurationExtensions
{
    public static IMasaConfigurationBuilder UseDcc(
        this IMasaConfigurationBuilder builder,
        Action<JsonSerializerOptions>? jsonSerializerOptions = null,
        Action<CallerOptions>? callerOptions = null,
        string sectionName = "DccOptions")
    {
        var configurationSection = builder.Configuration.GetSection(sectionName);
        var dccOptions = configurationSection.Get<DccOptions>();
        return builder.UseDcc(dccOptions, jsonSerializerOptions, callerOptions);
    }

    public static IMasaConfigurationBuilder UseDcc(
        this IMasaConfigurationBuilder builder,
        DccOptions dccOptions,
        Action<JsonSerializerOptions>? jsonSerializerOptions,
        Action<CallerOptions>? action)
    {
        var services = builder.Services;
        if (services.Any(service => service.ImplementationType == typeof(DccConfigurationProvider)))
            return builder;

        services.AddSingleton<DccConfigurationProvider>();

        services.AddMasaRedisCache(DEFAULT_CLIENT_NAME, dccOptions.RedisOptions)
            .AddSharedMasaMemoryCache(dccOptions.SubscribeKeyPrefix ?? DEFAULT_SUBSCRIBE_KEY_PREFIX);

        var dccConfigurationOptions = ComplementAndCheckDccConfigurationOption(builder, dccOptions);

        var jsonSerializerOption = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        jsonSerializerOptions?.Invoke(jsonSerializerOption);
        string callerName = DEFAULT_CLIENT_NAME;
        services.AddCaller(options =>
        {
            options.Assemblies = new[] { typeof(DccConfigurationProvider).Assembly };
            if (action == null)
            {
                options.UseHttpClient(callerName, ()
                    => new MasaHttpClientBuilder(
                        opt => opt.BaseAddress = new Uri(dccConfigurationOptions.ManageServiceAddress))
                );
            }
            else
            {
                action.Invoke(options);
                callerName = options.Callers.Select(opt => opt.Name).FirstOrDefault()
                    ?? throw new Exception("Missing Caller implementation, eg: options.UseHttpClient()");
            }
        });

        TryAddConfigurationApiClient(services,
            dccConfigurationOptions.DefaultSection,
            dccConfigurationOptions.ExpandSections,
            jsonSerializerOption);

        TryAddConfigurationApiManage(services,
            callerName,
            dccConfigurationOptions.DefaultSection,
            dccConfigurationOptions.ExpandSections);

        var serviceProvider = services.BuildServiceProvider();

        var configurationApiClient = serviceProvider.GetRequiredService<IConfigurationApiClient>();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        builder.AddRepository(new DccConfigurationRepository(new List<DccSectionOptions>
            {
                dccConfigurationOptions.DefaultSection
            }.Concat(dccConfigurationOptions.ExpandSections),
            configurationApiClient, loggerFactory));
        return builder;
    }

    public static IServiceCollection TryAddConfigurationApiClient(IServiceCollection services,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions> expansionSectionOptions,
        JsonSerializerOptions jsonSerializerOption)
    {
        services.TryAddSingleton(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMemoryCacheClientFactory>().CreateClient(DEFAULT_CLIENT_NAME);

            ArgumentNullException.ThrowIfNull(client, nameof(client));

            return DccFactory.CreateClient(
                serviceProvider,
                client,
                jsonSerializerOption,
                defaultSectionOption,
                expansionSectionOptions);
        });
        return services;
    }

    public static IServiceCollection TryAddConfigurationApiManage(IServiceCollection services,
        string callerName,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions> expansionSectionOptions)
    {
        services.TryAddSingleton(serviceProvider =>
        {
            var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
            return DccFactory.CreateManage(callerFactory.Create(callerName), defaultSectionOption, expansionSectionOptions);
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

        var distributedCacheClient = scope.ServiceProvider.GetDistributedCacheClient();
        dccConfigurationOptions.DefaultSection.ComplementAndCheckAppId(GetMasaAppConfigureOptions().AppId);
        dccConfigurationOptions.DefaultSection.ComplementAndCheckEnvironment(GetMasaAppConfigureOptions().Environment);
        dccConfigurationOptions.DefaultSection.ComplementAndCheckCluster(GetMasaAppConfigureOptions().Cluster);
        dccConfigurationOptions.DefaultSection.ComplementConfigObjects(distributedCacheClient);

        if (dccConfigurationOptions.ExpandSections.All(section => section.AppId != dccConfigurationOptions.PublicId))
        {
            var publicSection = new DccSectionOptions
            {
                AppId = dccConfigurationOptions.PublicId,
                Secret = dccConfigurationOptions.PublicSecret
            };
            dccConfigurationOptions.ExpandSections.Add(publicSection);
        }

        StaticConfig.AppId = dccConfigurationOptions.DefaultSection.AppId;
        StaticConfig.PublicId = dccConfigurationOptions.PublicId;

        if (dccConfigurationOptions.ExpandSections.Any(sectionOption
                => sectionOption.AppId == dccConfigurationOptions.DefaultSection.AppId))
            throw new ArgumentException("section repetition", nameof(dccConfigurationOptions.ExpandSections));

        foreach (var sectionOption in dccConfigurationOptions.ExpandSections)
        {
            sectionOption.ComplementAndCheckEnvironment(dccConfigurationOptions.DefaultSection.Environment);
            sectionOption.ComplementAndCheckCluster(dccConfigurationOptions.DefaultSection.Cluster);
            sectionOption.ComplementConfigObjects(distributedCacheClient);
        }
        return dccConfigurationOptions;

        MasaAppConfigureOptions GetMasaAppConfigureOptions()
        {
            return masaAppConfigureOptions ??= scope.ServiceProvider.GetRequiredService<IOptions<MasaAppConfigureOptions>>().Value;
        }
    }

    private static IDistributedCacheClient GetDistributedCacheClient(this IServiceProvider serviceProvider)
        => serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().CreateClient(DEFAULT_CLIENT_NAME);

    private static void CheckDccConfigurationOptions(DccConfigurationOptions dccOptions)
    {
        if (string.IsNullOrEmpty(dccOptions.ManageServiceAddress))
            throw new ArgumentNullException(nameof(dccOptions.ManageServiceAddress));

        if (!dccOptions.RedisOptions.Servers.Any() ||
            dccOptions.RedisOptions.Servers.Any(service => string.IsNullOrEmpty(service.Host) || service.Port <= 0))
            throw new ArgumentException(nameof(dccOptions.RedisOptions.Servers));

        if (dccOptions.ExpandSections.Any(dccSectionOptions => string.IsNullOrWhiteSpace(dccSectionOptions.AppId)))
            throw new ArgumentException("sections with an empty AppId are not allowed", nameof(dccOptions.ExpandSections));

        if (dccOptions.ExpandSections.DistinctBy(dccSectionOptions => dccSectionOptions.AppId).Count() != dccOptions.ExpandSections.Count)
            throw new ArgumentException("section repetition", nameof(dccOptions.ExpandSections));
    }

    private static ICachingBuilder AddSharedMasaMemoryCache(this ICachingBuilder builder, string subscribeKeyPrefix)
    {
        builder.AddMasaMemoryCache(options =>
        {
            options.SubscribeKeyType = SubscribeKeyTypes.SpecificPrefix;
            options.SubscribeKeyPrefix = subscribeKeyPrefix;
        });

        return builder;
    }

    private sealed class DccConfigurationProvider
    {

    }
}
