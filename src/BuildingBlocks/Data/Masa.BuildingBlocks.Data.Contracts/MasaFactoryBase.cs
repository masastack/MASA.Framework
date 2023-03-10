// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

public abstract class MasaFactoryBase<TService, TRelationOptions> : IMasaFactory<TService>
    where TService : class
    where TRelationOptions : MasaRelationOptions<TService>
{
    protected abstract string DefaultServiceNotFoundMessage { get; }
    protected abstract string SpecifyServiceNotFoundMessage { get; }
    protected abstract MasaFactoryOptions<TRelationOptions> FactoryOptions { get; }

    protected readonly IServiceProvider TransientServiceProvider;

    private IServiceProvider? _scopedServiceProvider;

    protected IServiceProvider ScopedServiceProvider
        => _scopedServiceProvider ??= TransientServiceProvider.GetRequiredService<ServiceScoped>().ServiceProvider;

    private IServiceProvider? _singletonServiceProvider;

    protected IServiceProvider SingletonServiceProvider
        => _singletonServiceProvider ??= TransientServiceProvider.GetRequiredService<ServiceSingleton>().ServiceProvider;

    protected MasaFactoryBase(IServiceProvider serviceProvider)
    {
        TransientServiceProvider = serviceProvider;
    }

    protected virtual IServiceProvider GetServiceProvider(string name) => SingletonServiceProvider;

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

        return defaultOptions.Func.Invoke(GetServiceProvider(defaultOptions.Name));
    }

    public virtual TService Create(string name)
    {
        if (TryCreate(name, out TService? services))
            return services;

        throw new NotSupportedException(string.Format(SpecifyServiceNotFoundMessage, name));
    }

    public bool TryCreate(string name, [NotNullWhen(true)] out TService? service)
    {
        var options = FactoryOptions.Options.SingleOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (options != null)
        {
            service = options.Func.Invoke(GetServiceProvider(name));
            return true;
        }
        service = null;
        return false;
    }
}
