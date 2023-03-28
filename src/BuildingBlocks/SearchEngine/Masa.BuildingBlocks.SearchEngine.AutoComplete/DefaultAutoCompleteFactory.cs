// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete;

public class DefaultAutoCompleteFactory : MasaFactoryBase<IAutoCompleteClient, MasaRelationOptions<IAutoCompleteClient>>, IAutoCompleteFactory
{
    protected override string DefaultServiceNotFoundMessage => "No default AutoComplete found";
    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{0}] AutoComplete, it was not found";
    protected override MasaFactoryOptions<MasaRelationOptions<IAutoCompleteClient>> FactoryOptions => _options.CurrentValue;

    private readonly IOptionsMonitor<AutoCompleteFactoryOptions> _options;

    public DefaultAutoCompleteFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _options = serviceProvider.GetRequiredService<IOptionsMonitor<AutoCompleteFactoryOptions>>();
    }

    public IAutoCompleteClient CreateClient() => base.Create();

    public IAutoCompleteClient CreateClient(string name) => base.Create(name);
}
