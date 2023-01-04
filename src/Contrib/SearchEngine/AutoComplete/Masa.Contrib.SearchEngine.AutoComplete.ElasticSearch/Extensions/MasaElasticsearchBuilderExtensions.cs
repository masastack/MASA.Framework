// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete;

public static class MasaElasticsearchBuilderExtensions
{
    public static MasaElasticsearchBuilder AddAutoComplete(this MasaElasticsearchBuilder builder)
        => builder.AddAutoComplete<Guid>();

    public static MasaElasticsearchBuilder AddAutoComplete<TValue>(
        this MasaElasticsearchBuilder builder) where TValue : notnull
    {
        var indexName = builder.ElasticClient.ConnectionSettings.DefaultIndex;
        if (string.IsNullOrEmpty(indexName))
            throw new ArgumentNullException(
                nameof(builder.ElasticClient.ConnectionSettings.DefaultIndex),
                "The default IndexName is not set");

        return builder.AddAutoComplete<AutoCompleteDocument<TValue>>(option => option.UseIndexName(indexName));
    }

    public static MasaElasticsearchBuilder AddAutoComplete(this MasaElasticsearchBuilder builder,
        Action<AutoCompleteOptions<AutoCompleteDocument<Guid>>>? action)
        => builder.AddAutoComplete<Guid>(action);

    public static MasaElasticsearchBuilder AddAutoComplete<TValue>(
        this MasaElasticsearchBuilder builder,
        Action<AutoCompleteOptions<AutoCompleteDocument<TValue>>>? action) where TValue : notnull
        => builder.AddAutoCompleteBySpecifyDocument(action);

    [Obsolete($"{nameof(AddAutoComplete)} expired, please use {nameof(AddAutoCompleteBySpecifyDocument)}")]
    public static MasaElasticsearchBuilder AddAutoComplete<TDocument, TValue>(
        this MasaElasticsearchBuilder builder)
        where TDocument : AutoCompleteDocument
        where TValue : notnull
        => builder.AddAutoCompleteBySpecifyDocument<TDocument>();

    [Obsolete($"{nameof(AddAutoComplete)} expired, please use {nameof(AddAutoCompleteBySpecifyDocument)}")]
    public static MasaElasticsearchBuilder AddAutoComplete<TDocument, TValue>(
        this MasaElasticsearchBuilder builder,
        Action<AutoCompleteOptions<TDocument>>? action)
        where TDocument : AutoCompleteDocument
        where TValue : notnull
        => builder.AddAutoCompleteBySpecifyDocument(action);

    public static MasaElasticsearchBuilder AddAutoCompleteBySpecifyDocument<TDocument>(
        this MasaElasticsearchBuilder builder)
        where TDocument : AutoCompleteDocument
    {
        var indexName = builder.ElasticClient.ConnectionSettings.DefaultIndex;
        if (string.IsNullOrEmpty(indexName))
            throw new ArgumentNullException(
                nameof(builder.ElasticClient.ConnectionSettings.DefaultIndex),
                "The default IndexName is not set");

        return builder.AddAutoCompleteBySpecifyDocument<TDocument>(option => option.UseIndexName(indexName));
    }

    public static MasaElasticsearchBuilder AddAutoCompleteBySpecifyDocument<TDocument>(
        this MasaElasticsearchBuilder builder,
        Action<AutoCompleteOptions<TDocument>>? action)
        where TDocument : AutoCompleteDocument
    {
        var options = new AutoCompleteOptions<TDocument>();
        if (!builder.IsSupportUpdate && action != null)
            action.Invoke(options);
        builder.Services.AddAutoCompleteCore(builder.Name, relationsOptions =>
        {
            relationsOptions.Func = serviceProvider =>
            {
                if (builder.IsSupportUpdate)
                {
                    options = new AutoCompleteOptions<TDocument>();
                    action?.Invoke(options);
                    return new AutoCompleteClient(
                        serviceProvider.GetRequiredService<IElasticClientFactory>().Create(relationsOptions.Name),
                        serviceProvider.GetRequiredService<IMasaElasticClientFactory>().Create(relationsOptions.Name),
                        options.IndexName,
                        options.DefaultOperator,
                        options.DefaultSearchType,
                        options.EnableMultipleCondition,
                        options.QueryAnalyzer
                    );
                }
                return new AutoCompleteClient(
                    builder.ElasticClient,
                    builder.Client,
                    options.IndexName,
                    options.DefaultOperator,
                    options.DefaultSearchType,
                    options.EnableMultipleCondition,
                    options.QueryAnalyzer
                );
            };
        }, builder.IsSupportUpdate ? ServiceLifetime.Scoped : ServiceLifetime.Singleton);
        return builder;
    }
}
