// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

internal class MasaUnnamedOptionsProvider<TOptions> where TOptions : class
{
    private readonly MasaUnnamedOptionsCache<TOptions> _optionsCache;

    public MasaUnnamedOptionsProvider(MasaUnnamedOptionsCache<TOptions> optionsCache)
    {
        _optionsCache = optionsCache;
    }

    public TOptions GetOptions(IServiceProvider serviceProvider)
    {
        return _optionsCache.GetOrAdd(serviceProvider, sp =>
        {
            var factory = sp.GetRequiredService<IOptionsFactory<TOptions>>();
            return factory.Create(Options.DefaultName);
        });
    }
}
