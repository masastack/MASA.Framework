// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Isolation;

internal static class MasaConfigurationBuilderExtensions
{
    public static IMasaConfigurationBuilder UseDccIsolation(this IMasaConfigurationBuilder builder)
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        var dccOptions = builder.Services.GetDccOptions();
        MasaArgumentException.ThrowIfNull(dccOptions);
        var dccConfigurationOptions = ComplementDccConfigurationOption(builder, dccOptions);

        var configurationApiClient = serviceProvider.GetRequiredService<IConfigurationApiClient>();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var source = new MasaConfigurationIsolationSource(new DccConfigurationIsolationRepository(dccConfigurationOptions.GetAllSections(),
            configurationApiClient, loggerFactory));

        var configurationBuilder = serviceProvider.GetService<IConfiguration>() as IConfigurationBuilder;
        configurationBuilder.AddConfiguration(builder.Add(source).Build());
        return builder;
    }

    public static DccConfigurationOptions ComplementDccConfigurationOption(
        IMasaConfigurationBuilder builder,
        DccOptions dccOptions)
    {
        DccConfigurationOptions dccConfigurationOptions = dccOptions;
        var serviceProvider = builder.Services.BuildServiceProvider();
        var environmentProvider = serviceProvider.GetRequiredService<EnvironmentProvider>();
        var cacheClient = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create("masa.contrib.configuration.configurationapi.dcc");
        var masaAppConfigureOptions = serviceProvider.GetRequiredService<IOptions<MasaAppConfigureOptions>>().Value;

        dccConfigurationOptions.PublicId = masaAppConfigureOptions.GetValue(nameof(DccOptions.PublicId), () => DccConsts.PUBLIC_ID);
        dccConfigurationOptions.PublicSecret = masaAppConfigureOptions.GetValue(nameof(DccOptions.PublicSecret));

        dccConfigurationOptions.DefaultSection.ComplementAndCheckAppId(masaAppConfigureOptions.AppId);
        dccConfigurationOptions.DefaultSection.ComplementAndCheckEnvironment(masaAppConfigureOptions.Environment);
        dccConfigurationOptions.DefaultSection.ComplementAndCheckCluster(masaAppConfigureOptions.Cluster);
        //dccConfigurationOptions.DefaultSection.ComplementConfigObjects(cacheClient);

        foreach (var environment in environmentProvider.GetEnvionments())
        {
            if (environment != dccConfigurationOptions.DefaultSection.Environment &&
                !dccConfigurationOptions.ExpandSections.Exists(section => section.AppId.Equals(dccConfigurationOptions.DefaultSection.AppId)
                && section.Environment.Equals(environment)))
            {
                var environmentAppSection = new DccSectionOptions
                {
                    AppId = dccConfigurationOptions.DefaultSection.AppId,
                    Secret = dccConfigurationOptions.DefaultSection.Secret,
                    Environment = environment,
                    Cluster = dccConfigurationOptions.DefaultSection.Cluster
                };
                //environmentAppSection.ComplementConfigObjects(cacheClient);
                dccConfigurationOptions.ExpandSections.Add(environmentAppSection);
            }
            if (!dccConfigurationOptions.ExpandSections.Exists(section => section.AppId.Equals(dccConfigurationOptions.PublicId) && section.Environment.Equals(environment)))
            {
                var publicSection = new DccSectionOptions
                {
                    AppId = dccConfigurationOptions.PublicId,
                    Secret = dccConfigurationOptions.PublicSecret,
                    Environment = environment,
                    Cluster = dccConfigurationOptions.DefaultSection.Cluster
                };
                //publicSection.ComplementConfigObjects(cacheClient);
                dccConfigurationOptions.ExpandSections.Add(publicSection);
            }
        }
        return dccConfigurationOptions;
    }
}
