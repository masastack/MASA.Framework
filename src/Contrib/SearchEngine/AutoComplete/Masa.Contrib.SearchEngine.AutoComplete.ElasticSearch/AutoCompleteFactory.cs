// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.SearchEngine.AutoComplete.ElasticSearch;

public class AutoCompleteFactory : MasaFactoryBase<IAutoCompleteClient, AutoCompleteRelationsOptions>, IAutoCompleteFactory
{
    // private readonly List<AutoCompleteRelations> _relations;
    // private readonly IElasticClientFactory _elasticClientFactory;
    // private readonly IMasaElasticClientFactory _masaElasticClientFactory;
    //
    // public AutoCompleteFactory(
    //     AutoCompleteRelationsOptions options,
    //     IElasticClientFactory elasticClientFactory,
    //     IMasaElasticClientFactory masaElasticClientFactory)
    // {
    //     _relations = options.Relations;
    //     _elasticClientFactory = elasticClientFactory;
    //     _masaElasticClientFactory = masaElasticClientFactory;
    // }
    //
    // public IAutoCompleteClient CreateClient()
    // {
    //     var item = _relations.SingleOrDefault(r => r.IsDefault) ?? _relations.FirstOrDefault();
    //     MasaArgumentException.ThrowIfNull(item);
    //     return new AutoCompleteClient(
    //         item.ElasticClient,
    //         item.MasaElasticClient,
    //         item.RealIndexName,
    //         item.DefaultOperator,
    //         item.DefaultSearchType,
    //         item.EnableMultipleCondition,
    //         item.QueryAnalyzer);
    // }

    // /// <summary>
    // /// Create a client corresponding to the index
    // /// </summary>
    // /// <param name="name">indexName or alias</param>
    // /// <returns></returns>
    // public IAutoCompleteClient CreateClient(string name)
    // {
    //     var item = _relations.FirstOrDefault(relation => relation.IndexName == name || relation.Alias == name);
    //     ArgumentNullException.ThrowIfNull(item, nameof(name));
    //     return new AutoCompleteClient(
    //         item.ElasticClient,
    //         item.MasaElasticClient,
    //         item.RealIndexName,
    //         item.DefaultOperator,
    //         item.DefaultSearchType,
    //         item.EnableMultipleCondition,
    //         item.QueryAnalyzer);
    // }

    protected override string DefaultServiceNotFoundMessage => "No default AutoComplete found, you may need service.AddCaller()";
    protected override string SpecifyServiceNotFoundMessage => "";
    protected override MasaFactoryOptions<AutoCompleteRelationsOptions> FactoryOptions => _optionsMonitor.CurrentValue;

    private readonly IOptionsMonitor<AutoCompleteFactoryOptions> _optionsMonitor;

    public AutoCompleteFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<AutoCompleteFactoryOptions>>();
    }

    public IAutoCompleteClient CreateClient() => base.Create();

    public IAutoCompleteClient CreateClient(string name) => base.Create(name);
}
