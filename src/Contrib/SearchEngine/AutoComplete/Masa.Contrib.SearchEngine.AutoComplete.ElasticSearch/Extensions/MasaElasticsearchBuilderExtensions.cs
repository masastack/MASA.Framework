// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete;

public static class MasaElasticsearchBuilderExtensions
{
    private static readonly string DefaultName = Microsoft.Extensions.Options.Options.DefaultName;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static MasaElasticsearchBuilder AddAutoComplete(this MasaElasticsearchBuilder builder)
        => builder.AddAutoComplete(DefaultName);

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static MasaElasticsearchBuilder AddAutoComplete(this MasaElasticsearchBuilder builder,
        string name)
        => builder.AddAutoComplete<Guid>(name);

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static MasaElasticsearchBuilder AddAutoComplete<TValue>(
        this MasaElasticsearchBuilder builder) where TValue : notnull
        => builder.AddAutoComplete<TValue>(DefaultName);

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static MasaElasticsearchBuilder AddAutoComplete<TValue>(
        this MasaElasticsearchBuilder builder,
        string name) where TValue : notnull
    {
        var indexName = builder.ElasticClient.ConnectionSettings.DefaultIndex;
        MasaArgumentException.ThrowIfNullOrWhiteSpace(indexName);

        return builder.AddAutoComplete<AutoCompleteDocument<TValue>>(name, option => option.UseIndexName(indexName));
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static MasaElasticsearchBuilder AddAutoComplete(this MasaElasticsearchBuilder builder,
        Action<AutoCompleteOptions<AutoCompleteDocument<Guid>>>? action)
        => builder.AddAutoComplete(DefaultName, action);

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static MasaElasticsearchBuilder AddAutoComplete(this MasaElasticsearchBuilder builder,
        string name,
        Action<AutoCompleteOptions<AutoCompleteDocument<Guid>>>? action)
        => builder.AddAutoComplete<Guid>(name, action);

    public static MasaElasticsearchBuilder AddAutoComplete<TValue>(
        this MasaElasticsearchBuilder builder,
        Action<AutoCompleteOptions<AutoCompleteDocument<TValue>>>? action) where TValue : notnull
        => builder.AddAutoComplete(DefaultName, action);

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static MasaElasticsearchBuilder AddAutoComplete<TValue>(
        this MasaElasticsearchBuilder builder,
        string name,
        Action<AutoCompleteOptions<AutoCompleteDocument<TValue>>>? action) where TValue : notnull
        => builder.AddAutoCompleteBySpecifyDocument(name, action);

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static MasaElasticsearchBuilder AddAutoCompleteBySpecifyDocument<TDocument>(
        this MasaElasticsearchBuilder builder)
        where TDocument : AutoCompleteDocument
        => builder.AddAutoCompleteBySpecifyDocument<TDocument>(DefaultName);

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static MasaElasticsearchBuilder AddAutoCompleteBySpecifyDocument<TDocument>(
        this MasaElasticsearchBuilder builder,
        string name)
        where TDocument : AutoCompleteDocument
    {
        var indexName = builder.ElasticClient.ConnectionSettings.DefaultIndex;

        MasaArgumentException.ThrowIfNullOrWhiteSpace(indexName);

        return builder.AddAutoCompleteBySpecifyDocument<TDocument>(name, option => option.UseIndexName(indexName));
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static MasaElasticsearchBuilder AddAutoCompleteBySpecifyDocument<TDocument>(
        this MasaElasticsearchBuilder builder,
        Action<AutoCompleteOptions<TDocument>>? action)
        where TDocument : AutoCompleteDocument
        => builder.AddAutoCompleteBySpecifyDocument(DefaultName, action);

    public static MasaElasticsearchBuilder AddAutoCompleteBySpecifyDocument<TDocument>(
        this MasaElasticsearchBuilder builder,
        string name,
        Action<AutoCompleteOptions<TDocument>>? action)
        where TDocument : AutoCompleteDocument
    {
        var options = new AutoCompleteOptions<TDocument>();
        if (!builder.AlwaysGetNewestElasticClient && action != null)
        {
            action.Invoke(options);
            MasaArgumentException.ThrowIfNullOrWhiteSpace(options.IndexName);
        }
        builder.Services.AddAutoCompleteCore(name, relationsOptions =>
        {
            relationsOptions.Func = serviceProvider =>
            {
                if (builder.AlwaysGetNewestElasticClient)
                {
                    options = new AutoCompleteOptions<TDocument>();
                    action?.Invoke(options);
                    return new AutoCompleteClient<TDocument>(
                        serviceProvider.GetRequiredService<IElasticClientFactory>().Create(relationsOptions.Name),
                        serviceProvider.GetRequiredService<IMasaElasticClientFactory>().Create(relationsOptions.Name),
                        serviceProvider.GetService<ILogger<AutoCompleteClient<TDocument>>>(),
                        options
                    );
                }

                return new AutoCompleteClient<TDocument>(
                    builder.ElasticClient,
                    builder.Client,
                    serviceProvider.GetService<ILogger<AutoCompleteClient<TDocument>>>(),
                    options
                );
            };
        }, builder.AlwaysGetNewestElasticClient ? ServiceLifetime.Scoped : ServiceLifetime.Singleton);
        return builder;
    }
}
