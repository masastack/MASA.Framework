// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

internal class MasaConfigureNamedOptions<TOptions> : IConfigureNamedOptions<TOptions> where TOptions : class
{
    private readonly Lazy<MasaConfigureNamedOptionsProvider> _masaConfigureNamedOptionsProviderLazy;
    private MasaConfigureNamedOptionsProvider MasaConfigureNamedOptionsProvider => _masaConfigureNamedOptionsProviderLazy.Value;

    public MasaConfigureNamedOptions(IServiceProvider serviceProvider)
    {
        _masaConfigureNamedOptionsProviderLazy = new Lazy<MasaConfigureNamedOptionsProvider>(serviceProvider.GetRequiredService<MasaConfigureNamedOptionsProvider>);
    }

    public void Configure(TOptions options) => Configure(Options.DefaultName, options);

    public void Configure(string name, TOptions options) => MasaConfigureNamedOptionsProvider.Configure(name, options);
}
