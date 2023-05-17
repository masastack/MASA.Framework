// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Configuration.ConfigurationApi.Dcc.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;

internal static class DccConfigurationOptionsExtensions
{
    public static void CheckAndComplementDccConfigurationOptions(
        this DccConfigurationOptions dccConfigurationOptions,
        IServiceProvider serviceProvider)
    {
        dccConfigurationOptions.CheckDccConfigurationOptions();

        dccConfigurationOptions.ComplementDccConfigurationOptions(serviceProvider);
    }

    public static void CheckDccConfigurationOptions(this DccConfigurationOptions dccConfigurationOptions)
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(dccConfigurationOptions.ManageServiceAddress);

        MasaArgumentException.ThrowIfNullOrEmptyCollection(dccConfigurationOptions.RedisOptions.Servers,
            nameof(dccConfigurationOptions.RedisOptions));

        dccConfigurationOptions.RedisOptions.Servers.ForEach(redisServerOption =>
        {
            MasaArgumentException.ThrowIfNullOrWhiteSpace(redisServerOption.Host, "Redis Host");

            MasaArgumentException.ThrowIfLessThanOrEqual(redisServerOption.Port, 0, "Redis Port");
        });

        dccConfigurationOptions.ExpandSections.ForEach(section =>
        {
            MasaArgumentException.ThrowIfNullOrWhiteSpace(section.AppId);
        });

        if (dccConfigurationOptions.ExpandSections.DistinctBy(sections => sections.AppId).Count() !=
            dccConfigurationOptions.ExpandSections.Count)
            throw new ArgumentException("AppId cannot be repeated", nameof(dccConfigurationOptions));

        if (dccConfigurationOptions is { EnablePublicConfig: true, PublicConfig: not null } &&
            dccConfigurationOptions.ExpandSections.Any(options => options.AppId == dccConfigurationOptions.PublicConfig.AppId))
            throw new MasaException($"Duplicate AppId, AppId: {dccConfigurationOptions.PublicConfig.AppId}");
    }

    public static void ComplementDccConfigurationOptions(
        this DccConfigurationOptions dccConfigurationOptions,
        IServiceProvider serviceProvider)
    {
        MasaAppConfigureOptions? masaAppConfigureOptions = null;
        IManualDistributedCacheClient? distributedCacheClient = null;

        dccConfigurationOptions.DefaultSection.ComplementDccSectionOptions(
            GetMasaAppConfigureOptions,
            GetDistributedCacheClient,
            GetDefaultEnvironment,
            () => GetMasaAppConfigureOptions().Cluster,
            () => dccConfigurationOptions.ConfigObjectSecret,
            false);

        dccConfigurationOptions.ComplementPublicConfig(GetDistributedCacheClient,
            GetDefaultEnvironment,
            () => GetMasaAppConfigureOptions().Cluster,
            () => dccConfigurationOptions.ConfigObjectSecret);

        if (dccConfigurationOptions.ExpandSections.Any(sectionOption
                => sectionOption.AppId == dccConfigurationOptions.DefaultSection.AppId))
            throw new Exception(
                $"The extension AppId cannot be the same as the default AppId, the repeated AppId is [{dccConfigurationOptions.DefaultSection.AppId}]");

        foreach (var sectionOption in dccConfigurationOptions.ExpandSections)
        {
            sectionOption.ComplementDccSectionOptions(
                GetMasaAppConfigureOptions,
                GetDistributedCacheClient,
                GetDefaultEnvironment,
                () => dccConfigurationOptions.DefaultSection.Cluster,
                () => dccConfigurationOptions.ConfigObjectSecret,
                true);
        }

        MasaAppConfigureOptions GetMasaAppConfigureOptions()
            => masaAppConfigureOptions ??= serviceProvider.GetRequiredService<IOptions<MasaAppConfigureOptions>>().Value;

        IManualDistributedCacheClient GetDistributedCacheClient()
            => distributedCacheClient ??= CacheUtils.CreateDistributedCacheClient(dccConfigurationOptions, serviceProvider);

        string GetDefaultEnvironment()
        {
            var masaConfigurationEnvironmentProvider = serviceProvider.GetRequiredService<MasaConfigurationEnvironmentProvider>();
            if (masaConfigurationEnvironmentProvider.TryGetDefaultEnvironment(serviceProvider, out var environment))
                return environment;

            return string.Empty;
        }
    }

    public static void ComplementPublicConfig(
        this DccConfigurationOptions dccConfigurationOptions,
        Func<IDistributedCacheClient> distributedCacheClientSetter,
        Func<string> defaultEnvironmentSetter,
        Func<string> defaultClusterSetter,
        Func<string?> defaultSecretSetter)
    {
        if (!dccConfigurationOptions.EnablePublicConfig)
            return;

        dccConfigurationOptions.PublicConfig ??= new PublicConfigOptions();
        dccConfigurationOptions.PublicConfig.ComplementDccConfigurationOptions(
            distributedCacheClientSetter,
            defaultEnvironmentSetter,
            defaultClusterSetter,
            defaultSecretSetter);
    }
}
