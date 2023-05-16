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
        bool skipComplementAppId)
    {
        MasaAppConfigureOptions? masaAppConfigureOptions = null;

        if (!skipComplementAppId) dccSectionOptions.ComplementAppId(() => GetMasaAppConfigureOptions().AppId);

        dccSectionOptions.ComplementEnvironment(defaultEnvironmentSetter);
        dccSectionOptions.ComplementCluster(defaultClusterSetter);
        dccSectionOptions.ComplementConfigObjects(() =>
        {
            var distributedCacheClient = distributedCacheClientSetter.Invoke();
            return distributedCacheClient.GetAllConfigObjects(dccSectionOptions.AppId, dccSectionOptions.Environment, dccSectionOptions.Cluster);
        });

        MasaAppConfigureOptions GetMasaAppConfigureOptions() => masaAppConfigureOptions ??= globalConfigureSetter.Invoke();
    }

    public static void ComplementAppId(this DccSectionOptions dccSectionOptions, Func<string> setter)
    {
        if (!dccSectionOptions.AppId.IsNullOrWhiteSpace())
            return;

        dccSectionOptions.AppId = setter.Invoke();
    }

    public static void ComplementEnvironment(this DccSectionOptions dccSectionOptions, Func<string> setter)
    {
        if (!dccSectionOptions.Environment.IsNullOrWhiteSpace())
            return;

        dccSectionOptions.Environment = setter.Invoke();
    }

    public static void ComplementCluster(this DccSectionOptions dccSectionOptions, Func<string> setter)
    {
        if (!dccSectionOptions.Cluster.IsNullOrWhiteSpace())
            return;

        dccSectionOptions.Cluster = setter.Invoke();
    }

    public static void ComplementConfigObjects(this DccSectionOptions dccSectionOptions, Func<List<string>> setter)
    {
        if (dccSectionOptions.ConfigObjects.Any())
            return;

        dccSectionOptions.ConfigObjects = setter.Invoke();
    }
}
