// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.BuildingBlocks.Caching;

public class DefaultTypeAliasFactory : MasaFactoryBase<ITypeAliasProvider, TypeAliasRelationOptions>, ITypeAliasFactory
{
    protected override string DefaultServiceNotFoundMessage => "Default TypeAlias not found";

    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{0}] TypeAlias, it was not found";

    protected override MasaFactoryOptions<TypeAliasRelationOptions> FactoryOptions => _optionsMonitor.Value;

    private readonly IOptions<TypeAliasFactoryOptions> _optionsMonitor;

    public DefaultTypeAliasFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _optionsMonitor = serviceProvider.GetRequiredService<IOptions<TypeAliasFactoryOptions>>();
    }
}
