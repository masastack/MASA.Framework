// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Configuration.ConfigurationApi.Dcc.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;

internal static class DccConfigurationOptionsExtensions
{
    private static readonly Lazy<string> DefaultAppIdLazy = new(() => (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Name!.Replace(".", "-"));
    private static string DefaultAppId => DefaultAppIdLazy.Value;

    public static void CheckAndComplementDccConfigurationOptions(
        this DccConfigurationOptions dccConfigurationOptions,
        string currentEnvironment,
        IServiceProvider serviceProvider)
    {
        dccConfigurationOptions.CheckDccConfigurationOptions();

        dccConfigurationOptions.ComplementDccConfigurationOptions(currentEnvironment, serviceProvider);
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
        string currentEnvironment,
        IServiceProvider serviceProvider)
    {
        IMasaConfiguration? masaConfiguration = null;
        IManualDistributedCacheClient? distributedCacheClient = null;

        dccConfigurationOptions.DefaultSection.ComplementDccSectionOptions(
            GetDistributedCacheClient,
            GetDefaultAppId,
            GetDefaultEnvironment,
            GetDefaultCluster,
            () => dccConfigurationOptions.ConfigObjectSecret,
            false);

        dccConfigurationOptions.ComplementPublicConfig(GetDistributedCacheClient,
            GetDefaultEnvironment,
            GetDefaultCluster,
            () => dccConfigurationOptions.ConfigObjectSecret);

        if (dccConfigurationOptions.ExpandSections.Any(sectionOption
                => sectionOption.AppId == dccConfigurationOptions.DefaultSection.AppId))
            throw new Exception(
                $"The extension AppId cannot be the same as the default AppId, the repeated AppId is [{dccConfigurationOptions.DefaultSection.AppId}]");

        foreach (var sectionOption in dccConfigurationOptions.ExpandSections)
        {
            sectionOption.ComplementDccSectionOptions(
                GetDistributedCacheClient,
                GetDefaultAppId,
                GetDefaultEnvironment,
                GetDefaultCluster,
                () => dccConfigurationOptions.ConfigObjectSecret,
                true);
        }

        IManualDistributedCacheClient GetDistributedCacheClient()
            => distributedCacheClient ??= CacheUtils.CreateDistributedCacheClient(dccConfigurationOptions, serviceProvider);

        string GetDefaultAppId()
        {
            var appId = GetMasaConfiguration().Local.GetSection(nameof(MasaAppConfigureOptions.AppId)).Value;
            return !appId.IsNullOrWhiteSpace() ? appId : DefaultAppId;
        }

        string GetDefaultCluster()
        {
            var cluster = GetMasaConfiguration().Local.GetSection(nameof(MasaAppConfigureOptions.Cluster)).Value;
            return !cluster.IsNullOrWhiteSpace() ? cluster : ConfigurationConstant.DEFAULT_CLUSTER;
        }

        IMasaConfiguration GetMasaConfiguration()
        {
            if (masaConfiguration != null)
                return masaConfiguration;

            masaConfiguration = serviceProvider.GetRequiredService<IMasaConfigurationFactory>().Create(SectionTypes.Local);
            return masaConfiguration;
        }

        string GetDefaultEnvironment() => currentEnvironment;
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
