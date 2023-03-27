// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete;

public static class AutoCompleteOptionsExtensions
{
    public static ElasticSearchBuilder UseElasticSearch(
        this AutoCompleteOptionsBuilder autoCompleteOptionsBuilder,
        Action<ElasticSearchAutoCompleteOptions> action)
    {
        MasaArgumentException.ThrowIfNull(action);

        autoCompleteOptionsBuilder.Services.TryAddSingleton<IElasticClientProvider, DefaultElasticClientProvider>();
        autoCompleteOptionsBuilder.AddAutoComplete(serviceProvider =>
        {
            var elasticSearchAutoCompleteOptions = new ElasticSearchAutoCompleteOptions();
            action.Invoke(elasticSearchAutoCompleteOptions);
            MasaArgumentException.ThrowIfNullOrWhiteSpace(elasticSearchAutoCompleteOptions.IndexName);

            var elasticClientProvider = serviceProvider.GetRequiredService<IElasticClientProvider>();
            var item = elasticClientProvider.GetClient(elasticSearchAutoCompleteOptions.ElasticsearchOptions);
            return new AutoCompleteClient(
                item.ElasticClient,
                item.MasaElasticClient,
                serviceProvider.GetService<ILogger<AutoCompleteClient>>(),
                elasticSearchAutoCompleteOptions,
                autoCompleteOptionsBuilder.DocumentType,
                elasticClientProvider
            );
        });
        return new ElasticSearchBuilder();
    }
}
