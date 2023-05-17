// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;

internal static class DccSectionOptionsExtensions
{
    public static void ComplementDccSectionOptions(
        this DccSectionOptions dccSectionOptions,
        Func<MasaAppConfigureOptions> globalConfigureSetter,
        Func<IDistributedCacheClient> distributedCacheClientSetter,
        Func<string> defaultEnvironmentSetter,
        Func<string> defaultClusterSetter,
        Func<string?> defaultSecretSetter,
        bool skipComplementAppId)
    {
        MasaAppConfigureOptions? masaAppConfigureOptions = null;

        if (!skipComplementAppId) dccSectionOptions.ComplementAppId(() => GetMasaAppConfigureOptions().AppId);

        dccSectionOptions.ComplementDccSectionOptions(
            distributedCacheClientSetter,
            () => dccSectionOptions.AppId,
            defaultEnvironmentSetter,
            defaultClusterSetter,
            defaultSecretSetter
        );

        MasaAppConfigureOptions GetMasaAppConfigureOptions() => masaAppConfigureOptions ??= globalConfigureSetter.Invoke();
    }

    public static void ComplementDccConfigurationOptions(
        this PublicConfigOptions publicConfigOptions,
        Func<IDistributedCacheClient> distributedCacheClientSetter,
        Func<string> defaultEnvironmentSetter,
        Func<string> defaultClusterSetter,
        Func<string?> defaultSecretSetter)
        => publicConfigOptions.ComplementDccSectionOptions(
            distributedCacheClientSetter,
            () => publicConfigOptions.AppId,
            defaultEnvironmentSetter,
            defaultClusterSetter,
            defaultSecretSetter
        );

    private static void ComplementDccSectionOptions(
        this DccSectionOptionsBase dccSectionOptions,
        Func<IDistributedCacheClient> distributedCacheClientSetter,
        Func<string> defaultAppIdSetter,
        Func<string> defaultEnvironmentSetter,
        Func<string> defaultClusterSetter,
        Func<string?> defaultSecretSetter)
    {
        dccSectionOptions.ComplementEnvironment(defaultEnvironmentSetter);
        dccSectionOptions.ComplementCluster(defaultClusterSetter);
        dccSectionOptions.ComplementConfigObjects(() =>
        {
            var distributedCacheClient = distributedCacheClientSetter.Invoke();
            return distributedCacheClient.GetAllConfigObjects(defaultAppIdSetter.Invoke(), dccSectionOptions.Environment,
                dccSectionOptions.Cluster);
        });
        dccSectionOptions.ComplementSecret(defaultSecretSetter);
    }

    public static void ComplementAppId(this DccSectionOptions dccSectionOptions, Func<string> setter)
    {
        if (!dccSectionOptions.AppId.IsNullOrWhiteSpace())
            return;

        dccSectionOptions.AppId = setter.Invoke();
    }

    public static void ComplementEnvironment(this DccSectionOptionsBase dccSectionOptions, Func<string> setter)
    {
        if (!dccSectionOptions.Environment.IsNullOrWhiteSpace())
            return;

        dccSectionOptions.Environment = setter.Invoke();
    }

    public static void ComplementCluster(this DccSectionOptionsBase dccSectionOptions, Func<string> setter)
    {
        if (!dccSectionOptions.Cluster.IsNullOrWhiteSpace())
            return;

        dccSectionOptions.Cluster = setter.Invoke();
    }

    public static void ComplementConfigObjects(this DccSectionOptionsBase dccSectionOptions, Func<List<string>> setter)
    {
        if (dccSectionOptions.ConfigObjects.Any())
            return;

        dccSectionOptions.ConfigObjects = setter.Invoke();
    }

    public static void ComplementSecret(this DccSectionOptionsBase dccSectionOptions, Func<string?> setter)
    {
        if (!dccSectionOptions.Secret.IsNullOrWhiteSpace())
            return;

        dccSectionOptions.Secret = setter.Invoke();
    }
}
