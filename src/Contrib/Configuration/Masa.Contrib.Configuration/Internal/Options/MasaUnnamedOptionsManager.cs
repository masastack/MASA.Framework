// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

/// <summary>
/// When isolation is not enabled
/// </summary>
/// <typeparam name="TOptions"></typeparam>
internal sealed class MasaUnnamedOptionsManager<TOptions> :
    IOptions<TOptions>
    where TOptions : class
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MasaUnnamedOptionsProvider _masaUnnamedOptionsProvider;

    public MasaUnnamedOptionsManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _masaUnnamedOptionsProvider = serviceProvider.GetRequiredService<MasaUnnamedOptionsProvider>();
    }

    /// <summary>
    ///
    /// </summary>
    public TOptions Value => _masaUnnamedOptionsProvider.GetOptions<TOptions>(_serviceProvider);
}
