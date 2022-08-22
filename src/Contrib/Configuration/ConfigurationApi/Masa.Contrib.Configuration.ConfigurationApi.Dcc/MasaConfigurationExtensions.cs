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
        var dccOptions = configurationSection.Get<DccConfigurationOptions>();
        return builder.UseDcc(dccOptions, jsonSerializerOptions, callerOptions);
    }

    public static IMasaConfigurationBuilder UseDcc(
        this IMasaConfigurationBuilder builder,
        DccConfigurationOptions dccConfigurationOptions,
        Action<JsonSerializerOptions>? jsonSerializerOptions,
        Action<CallerOptions>? action)
    {
        var services = builder.Services;
        if (services.Any(service => service.ImplementationType == typeof(DccConfigurationProvider)))
            return builder;

        services.AddSingleton<DccConfigurationProvider>();
        builder.ComplementAndCheckDccConfigurationOption(dccConfigurationOptions);

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

        services.AddMasaRedisCache(DEFAULT_CLIENT_NAME, dccConfigurationOptions.RedisOptions)
            .AddSharedMasaMemoryCache(dccConfigurationOptions.SubscribeKeyPrefix ?? DEFAULT_SUBSCRIBE_KEY_PREFIX);

        TryAddConfigurationApiClient(services,
            dccConfigurationOptions.DefaultSection!,
            dccConfigurationOptions.ExpandSections!,
            jsonSerializerOption);

        TryAddConfigurationApiManage(services,
            callerName,
            dccConfigurationOptions.DefaultSection!,
            dccConfigurationOptions.ExpandSections!);

        var serviceProvider = services.BuildServiceProvider();

        var configurationApiClient = serviceProvider.GetRequiredService<IConfigurationApiClient>();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        builder.AddRepository(new DccConfigurationRepository(new List<DccSectionOptions>
            {
                dccConfigurationOptions.DefaultSection!
            }.Concat(dccConfigurationOptions.ExpandSections!),
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

    private static DccConfigurationOptions ComplementAndCheckDccConfigurationOption(
        this IMasaConfigurationBuilder builder,
        DccConfigurationOptions dccConfigurationOptions)
    {
        CheckDccConfigurationOptions(dccConfigurationOptions);

        var serviceProvider = builder.Services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var masaAppConfigureOptions = scope.ServiceProvider.GetRequiredService<IOptions<MasaAppConfigureOptions>>();
        dccConfigurationOptions.PublicId ??=
            masaAppConfigureOptions.Value.GetValue(nameof(DccConfigurationOptions.PublicId), () => DEFAULT_PUBLIC_ID);
        dccConfigurationOptions.PublicSecret ??=
            masaAppConfigureOptions.Value.GetValue(nameof(DccConfigurationOptions.PublicSecret), () => string.Empty);

        var distributedCacheClient = GetDistributedCacheClient(scope.ServiceProvider);
        bool isCheck = true;
        if (dccConfigurationOptions.DefaultSection == null)
        {
            dccConfigurationOptions.DefaultSection = new DccSectionOptions();
            isCheck = false;
        }
        dccConfigurationOptions.DefaultSection.ComplementAndCheckAppId(masaAppConfigureOptions.Value.AppId, isCheck);
        dccConfigurationOptions.DefaultSection.ComplementAndCheckEnvironment(masaAppConfigureOptions.Value.Environment, isCheck);
        dccConfigurationOptions.DefaultSection.ComplementAndCheckCluster(masaAppConfigureOptions.Value.Cluster, isCheck);
        dccConfigurationOptions.DefaultSection.ComplementConfigObjects(distributedCacheClient);

        dccConfigurationOptions.ExpandSections ??= new();

        if (!string.IsNullOrWhiteSpace(dccConfigurationOptions.PublicId) &&
            dccConfigurationOptions.ExpandSections.All(section => section.AppId != dccConfigurationOptions.PublicId))
        {
            var publicSection = new DccSectionOptions
            {
                AppId = dccConfigurationOptions.PublicId,
                Secret = dccConfigurationOptions.PublicSecret
            };
            publicSection.ComplementConfigObjects(distributedCacheClient);
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
    }

    private static IDistributedCacheClient GetDistributedCacheClient(IServiceProvider serviceProvider)
        => serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().CreateClient(DEFAULT_CLIENT_NAME);

    private static void CheckDccConfigurationOptions(DccConfigurationOptions dccConfigurationOptions)
    {
        ArgumentNullException.ThrowIfNull(dccConfigurationOptions, nameof(dccConfigurationOptions));

        if (string.IsNullOrEmpty(dccConfigurationOptions.ManageServiceAddress))
            throw new ArgumentNullException(nameof(dccConfigurationOptions.ManageServiceAddress));

        ArgumentNullException.ThrowIfNull(dccConfigurationOptions.RedisOptions, nameof(dccConfigurationOptions.RedisOptions));

        if (dccConfigurationOptions.RedisOptions.Servers == null || !dccConfigurationOptions.RedisOptions.Servers.Any() ||
            dccConfigurationOptions.RedisOptions.Servers.Any(service => string.IsNullOrEmpty(service.Host) || service.Port <= 0))
            throw new ArgumentNullException(nameof(dccConfigurationOptions.RedisOptions.Servers));

        if (dccConfigurationOptions.ExpandSections != null &&
            dccConfigurationOptions.ExpandSections.DistinctBy(dccSectionOptions => dccSectionOptions.AppId).Count() > 1)
            throw new ArgumentException("section repetition", nameof(dccConfigurationOptions.ExpandSections));
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
