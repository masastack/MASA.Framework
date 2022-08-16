// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public abstract class AbstractMasaFactory<TService, TFactoryOptions, TRelationOptions> : IMasaFactory<TService>
    where TService : class
    where TRelationOptions : MasaRelationOptions<TService>
    where TFactoryOptions : MasaFactoryOptions<TRelationOptions>
{
    protected abstract string DefaultServiceNotFoundMessage { get; }
    protected abstract string SpecifyServiceNotFoundMessage { get; }
    protected virtual IOptionsMonitor<TFactoryOptions> OptionsMonitor { get; }

    protected readonly IServiceProvider ServiceProvider;

    public AbstractMasaFactory(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        OptionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<TFactoryOptions>>();
    }

    private MasaRelationOptions<TService>? GetDefaultOptions(List<TRelationOptions> optionsList)
    {
        return optionsList.SingleOrDefault(c => c.Name == Options.DefaultName) ??
            optionsList.FirstOrDefault();
    }

    public virtual TService Create()
    {
        var defaultOptions = GetDefaultOptions(OptionsMonitor.CurrentValue.Options);
        if (defaultOptions == null)
            throw new NotImplementedException(DefaultServiceNotFoundMessage);

        return defaultOptions.Func.Invoke(ServiceProvider);
    }

    public virtual TService Create(string name)
    {
        var options = OptionsMonitor.CurrentValue.Options.SingleOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (options == null)
            throw new NotImplementedException(string.Format(SpecifyServiceNotFoundMessage, name));

        return options.Func.Invoke(ServiceProvider);
    }
}
