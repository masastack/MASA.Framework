// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

internal class MasaOptionsMonitor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TOptions> :
    IOptionsMonitor<TOptions>,
    IDisposable
    where TOptions : class
{
    private readonly Lazy<IOptionsMonitor<TOptions>> _optionsLazy;

    public MasaOptionsMonitor(IServiceProvider serviceProvider, MasaOptionsMonitorProvider masaOptionsMonitorProvider)
    {
        _optionsLazy = new Lazy<IOptionsMonitor<TOptions>>(() => masaOptionsMonitorProvider.GetOptionsMonitor<TOptions>(serviceProvider));
    }

    public TOptions Get(string name) => _optionsLazy.Value.Get(name);

    public IDisposable OnChange(Action<TOptions, string> listener) => _optionsLazy.Value.OnChange(listener);

    public TOptions CurrentValue => _optionsLazy.Value.CurrentValue;

    public void Dispose()
    {

    }
}
