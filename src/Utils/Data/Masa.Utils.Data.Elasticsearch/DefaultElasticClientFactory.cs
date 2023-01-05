// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch;

public class DefaultElasticClientFactory : IElasticClientFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptionsSnapshot<ElasticsearchFactoryOptions> _elasticsearchFactoryOptions;

    public DefaultElasticClientFactory(IServiceProvider serviceProvider,
        IOptionsSnapshot<ElasticsearchFactoryOptions> elasticsearchFactoryOptions)
    {
        _serviceProvider = serviceProvider;
        _elasticsearchFactoryOptions = elasticsearchFactoryOptions;
    }

    public IElasticClient Create()
    {
        var options = _elasticsearchFactoryOptions.Value;
        var defaultOptions = GetDefaultOptions(options.Options);
        if (defaultOptions == null)
            throw new NotSupportedException("No default ElasticClient found");

        return defaultOptions.Func.Invoke(_serviceProvider);
    }

    private static ElasticsearchRelationsOptions? GetDefaultOptions(List<ElasticsearchRelationsOptions> optionsList)
    {
        return optionsList.SingleOrDefault(c => c.Name == Microsoft.Extensions.Options.Options.DefaultName) ??
            optionsList.FirstOrDefault();
    }

    public IElasticClient Create(string name)
    {
        var options = _elasticsearchFactoryOptions.Value.Options.SingleOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (options == null)
            throw new NotSupportedException($"Please make sure you have used [{name}] ElasticClient, it was not found");

        return options.Func.Invoke(_serviceProvider);
    }
}
