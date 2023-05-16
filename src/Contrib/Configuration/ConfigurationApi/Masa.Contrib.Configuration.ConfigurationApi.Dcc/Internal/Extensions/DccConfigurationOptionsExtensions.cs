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
    }

    public static void ComplementDccConfigurationOptions(
        this DccConfigurationOptions dccConfigurationOptions,
        IServiceProvider serviceProvider)
    {
        MasaAppConfigureOptions? masaAppConfigureOptions = null;
        IDistributedCacheClient? distributedCacheClient = null;

        dccConfigurationOptions.DefaultSection.ComplementDccSectionOptions(
            GetMasaAppConfigureOptions,
            GetDistributedCacheClient,
            GetDefaultEnvironment,
            () => GetMasaAppConfigureOptions().Cluster,
            false);

        dccConfigurationOptions.ComplementPublicId(()
            => GetMasaAppConfigureOptions().GetValue(nameof(DccOptions.PublicId), () => DEFAULT_PUBLIC_ID));
        dccConfigurationOptions.ComplementPublicSecret(() => GetMasaAppConfigureOptions().GetValue(nameof(DccOptions.PublicSecret)));
        dccConfigurationOptions.ComplementExpandSections();

        DccConfig.AppId = dccConfigurationOptions.DefaultSection.AppId;
        DccConfig.PublicId = dccConfigurationOptions.PublicId!;

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
                true);
        }

        MasaAppConfigureOptions GetMasaAppConfigureOptions()
            => masaAppConfigureOptions ??= serviceProvider.GetRequiredService<IOptions<MasaAppConfigureOptions>>().Value;

        IDistributedCacheClient GetDistributedCacheClient()
            => distributedCacheClient ?? serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create(DEFAULT_CLIENT_NAME);

        string GetDefaultEnvironment()
        {
            var masaConfigurationEnvironmentProvider = serviceProvider.GetRequiredService<MasaConfigurationEnvironmentProvider>();
            if (masaConfigurationEnvironmentProvider.TryGetDefaultEnvironment(serviceProvider, out var environment))
                return environment;

            return string.Empty;
        }
    }

    public static void ComplementPublicId(this DccConfigurationOptions dccConfigurationOptions, Func<string> setter)
    {
        if (!dccConfigurationOptions.PublicId.IsNullOrWhiteSpace())
            return;

        dccConfigurationOptions.PublicId = setter.Invoke();
    }

    public static void ComplementPublicSecret(this DccConfigurationOptions dccConfigurationOptions, Func<string> setter)
    {
        if (!dccConfigurationOptions.PublicSecret.IsNullOrWhiteSpace())
            return;

        dccConfigurationOptions.PublicSecret = setter.Invoke();
    }

    public static void ComplementExpandSections(this DccConfigurationOptions dccConfigurationOptions)
    {
        if (dccConfigurationOptions.ExpandSections.All(section => section.AppId != dccConfigurationOptions.PublicId))
        {
            var publicSection = new DccSectionOptions
            {
                AppId = dccConfigurationOptions.PublicId!,
                Secret = dccConfigurationOptions.PublicSecret
            };
            dccConfigurationOptions.ExpandSections.Add(publicSection);
        }
    }
}
