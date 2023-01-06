// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public abstract class MasaFactoryBase<TService, TRelationOptions> : IMasaFactory<TService>
    where TService : class
    where TRelationOptions : MasaRelationOptions<TService>
{
    protected abstract string DefaultServiceNotFoundMessage { get; }
    protected abstract string SpecifyServiceNotFoundMessage { get; }
    protected abstract MasaFactoryOptions<TRelationOptions> FactoryOptions { get; }

    protected readonly IServiceProvider ServiceProvider;

    protected MasaFactoryBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    private static MasaRelationOptions<TService>? GetDefaultOptions(List<TRelationOptions> optionsList)
    {
        return optionsList.SingleOrDefault(c => c.Name == Options.DefaultName) ??
            optionsList.FirstOrDefault();
    }

    public virtual TService Create()
    {
        var defaultOptions = GetDefaultOptions(FactoryOptions.Options);
        if (defaultOptions == null)
            throw new NotSupportedException(DefaultServiceNotFoundMessage);

        return defaultOptions.Func.Invoke(ServiceProvider);
    }

    public virtual TService Create(string name)
    {
        var options = FactoryOptions.Options.SingleOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (options == null)
            throw new NotSupportedException(string.Format(SpecifyServiceNotFoundMessage, name));

        return options.Func.Invoke(ServiceProvider);
    }
}
