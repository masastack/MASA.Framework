// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete;

public class AutoCompleteFactory : MasaFactoryBase<IAutoCompleteClient, AutoCompleteRelationsOptions>, IAutoCompleteFactory
{
    protected override string DefaultServiceNotFoundMessage => "No default AutoComplete found";
    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{0}] AutoComplete, it was not found";
    protected override MasaFactoryOptions<AutoCompleteRelationsOptions> FactoryOptions => _optionsMonitor.CurrentValue;

    private readonly IOptionsMonitor<AutoCompleteFactoryOptions> _optionsMonitor;

    public AutoCompleteFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<AutoCompleteFactoryOptions>>();
    }

    public IAutoCompleteClient CreateClient() => base.Create();

    public IAutoCompleteClient CreateClient(string name) => base.Create(name);
}
