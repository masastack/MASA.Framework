// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

internal sealed class MasaUnnamedOptionsManager<TOptions> :
    IOptions<TOptions>
    where TOptions : class
{
    private readonly IServiceProvider _serviceProvider;

    public MasaUnnamedOptionsManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public TOptions Value
    {
        get
        {
            var masaUnnamedOptionsProvider = _serviceProvider.GetService<MasaUnnamedOptionsProvider<TOptions>>();
            if (masaUnnamedOptionsProvider != null)
                return masaUnnamedOptionsProvider.GetOptions(_serviceProvider);

            return _serviceProvider.GetRequiredService<UnnamedOptionsManager<TOptions>>().Value;
        }
    }
}
