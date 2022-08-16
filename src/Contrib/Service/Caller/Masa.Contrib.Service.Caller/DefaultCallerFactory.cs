// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller;

internal class DefaultCallerFactory : ICallerFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptionsMonitor<CallerFactoryOptions> _optionsMonitor;

    public DefaultCallerFactory(
        IServiceProvider serviceProvider,
        IOptionsMonitor<CallerFactoryOptions> optionsMonitor)
    {
        _serviceProvider = serviceProvider;
        _optionsMonitor = optionsMonitor;
    }

    private CallerRelationOptions? GetDefaultOptions(List<CallerRelationOptions> optionsList)
    {
        return optionsList.SingleOrDefault(c => c.Name == Options.DefaultName) ??
            optionsList.FirstOrDefault();
    }

    public ICaller Create()
    {
        var defaultOptions = GetDefaultOptions(_optionsMonitor.CurrentValue.Options);
        if (defaultOptions == null)
            throw new MasaException("No default Caller found, you may need service.AddCaller()");

        return defaultOptions.Func.Invoke(_serviceProvider);
    }

    public ICaller Create(string name)
    {
        var options = _optionsMonitor.CurrentValue.Options.SingleOrDefault(c => c.Name == name);
        if (options == null)
            throw new MasaException($"Please make sure you have used [{name}] Caller, it was not found");

        return options.Func.Invoke(_serviceProvider);
    }
}
