// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete;

public static class AutoCompleteOptionsExtensions
{
    public static ElasticSearchBuilder UseElasticSearch(
        this AutoCompleteOptionsBuilder autoCompleteOptions,
        Action<ElasticSearchAutoCompleteOptions> action)
    {
        MasaArgumentException.ThrowIfNull(action);

        autoCompleteOptions.Services.AddAutoComplete(autoCompleteOptions.Name, serviceProvider =>
        {
            var elasticSearchAutoCompleteOptions = new ElasticSearchAutoCompleteOptions();
            action.Invoke(elasticSearchAutoCompleteOptions);
            MasaArgumentException.ThrowIfNullOrWhiteSpace(elasticSearchAutoCompleteOptions.IndexName);

            var elasticClient = ElasticClientUtils.Create(elasticSearchAutoCompleteOptions.ElasticsearchOptions);
            return new AutoCompleteClient(
                elasticClient,
                new DefaultMasaElasticClient(elasticClient),
                serviceProvider.GetService<ILogger<AutoCompleteClient>>(),
                elasticSearchAutoCompleteOptions,
                autoCompleteOptions.DocumentType
            );
        });
        return new ElasticSearchBuilder();
    }
}
