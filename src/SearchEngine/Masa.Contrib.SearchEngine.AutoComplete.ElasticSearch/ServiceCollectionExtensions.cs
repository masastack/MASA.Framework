// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.SearchEngine.AutoComplete;

public static class ServiceCollectionExtensions
{
    public static MasaElasticsearchBuilder AddAutoComplete(this MasaElasticsearchBuilder builder)
        => builder.AddAutoComplete<Guid>();

    public static MasaElasticsearchBuilder AddAutoComplete<TValue>(
        this MasaElasticsearchBuilder builder) where TValue : notnull
        => builder.AddAutoComplete<AutoCompleteDocument<TValue>, TValue>();

    public static MasaElasticsearchBuilder AddAutoComplete<TDocument, TValue>(
        this MasaElasticsearchBuilder builder)
        where TDocument : AutoCompleteDocument<TValue> where TValue : notnull
    {
        var indexName = builder.ElasticClient.ConnectionSettings.DefaultIndex;
        if (string.IsNullOrEmpty(indexName))
            throw new ArgumentNullException(
                nameof(builder.ElasticClient.ConnectionSettings.DefaultIndex),
                "The default IndexName is not set");

        return builder.AddAutoComplete<TDocument, TValue>(option => option.UseIndexName(indexName));
    }

    public static MasaElasticsearchBuilder AddAutoComplete(
        this MasaElasticsearchBuilder builder,
        Action<AutoCompleteOptions<AutoCompleteDocument<long>, long>>? action)
        => builder.AddAutoComplete<long>(action);

    public static MasaElasticsearchBuilder AddAutoComplete<TValue>(
        this MasaElasticsearchBuilder builder,
        Action<AutoCompleteOptions<AutoCompleteDocument<TValue>, TValue>>? action) where TValue : notnull
        => builder.AddAutoComplete<AutoCompleteDocument<TValue>, TValue>(action);

    public static MasaElasticsearchBuilder AddAutoComplete<TDocument, TValue>(
        this MasaElasticsearchBuilder builder,
        Action<AutoCompleteOptions<TDocument, TValue>>? action)
        where TDocument : AutoCompleteDocument<TValue> where TValue : notnull
    {
        AutoCompleteOptions<TDocument, TValue> options = new AutoCompleteOptions<TDocument, TValue>();
        action?.Invoke(options);
        builder.Services.AddAutoCompleteCore(builder.ElasticClient, builder.Client, options);
        return builder;
    }

    private static void AddAutoCompleteCore<TDocument, TValue>(this IServiceCollection services,
        IElasticClient elasticClient,
        IMasaElasticClient client,
        AutoCompleteOptions<TDocument, TValue> option)
        where TDocument : AutoCompleteDocument<TValue> where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(services);

        ArgumentNullException.ThrowIfNull(option.IndexName, nameof(option.IndexName));

        services.TryAddSingleton(new AutoCompleteRelationsOptions());

        var autoCompleteRelations = new AutoCompleteRelations(elasticClient, client, option.IndexName, option.Alias, option.IsDefault,
            option.DefaultOperator, option.DefaultSearchType);
        services.TryAddAutoCompleteRelation(autoCompleteRelations);

        services.TryAddSingleton<IAutoCompleteFactory, AutoCompleteFactory>();
        services.TryAddSingleton(serviceProvider => serviceProvider.GetRequiredService<IAutoCompleteFactory>().CreateClient());
        client.TryCreateIndexAsync(services.BuildServiceProvider().GetService<ILogger<IAutoCompleteClient>>(), option);
    }

    private static void TryAddAutoCompleteRelation(this IServiceCollection services, AutoCompleteRelations relation)
    {
        var serviceProvider = services.BuildServiceProvider();
        var relationsOptions = serviceProvider.GetRequiredService<AutoCompleteRelationsOptions>();

        if (relationsOptions.Relations.Any(r => r.Alias == relation.Alias || r.IndexName == relation.IndexName))
            throw new ArgumentException($"indexName or alias exists");

        if (relation.IsDefault && relationsOptions.Relations.Any(r => r.IsDefault))
            throw new ArgumentException("ElasticClient can only have one default", nameof(ElasticsearchRelations.IsDefault));

        relationsOptions.AddRelation(relation);
    }

    private static void TryCreateIndexAsync<TDocument, TValue>(
        this IMasaElasticClient client,
        ILogger<IAutoCompleteClient>? logger,
        AutoCompleteOptions<TDocument, TValue> option)
        where TDocument : AutoCompleteDocument<TValue> where TValue : notnull
    {
        IAliases? aliases = null;
        if (option.Alias != null)
        {
            aliases = new Aliases();
            aliases.Add(option.Alias, new Alias());
        }

        var existsResponse = client.IndexExistAsync(option.IndexName, CancellationToken.None).ConfigureAwait(false).GetAwaiter()
            .GetResult();
        if (!existsResponse.IsValid || existsResponse.Exists)
        {
            if (!existsResponse.IsValid)
                logger?.LogError("AutoComplete: Initialization index is abnormal, {Message}", existsResponse.Message);

            return;
        }

        client.CreateIndex(logger, option.IndexName, aliases, option);
    }

    private static void CreateIndex<TDocument, TValue>(
        this IMasaElasticClient client,
        ILogger<IAutoCompleteClient>? logger,
        string indexName,
        IAliases? aliases,
        AutoCompleteOptions<TDocument, TValue> option)
        where TDocument : AutoCompleteDocument<TValue> where TValue : notnull
    {
        IAnalysis analysis = new AnalysisDescriptor();
        analysis.Analyzers = new Analyzers();
        analysis.TokenFilters = new TokenFilters();
        IIndexSettings indexSettings = new IndexSettings()
        {
            Analysis = analysis
        };
        string analyzer = "ik_max_word_pinyin";
        if (option.IndexSettingAction != null)
            option.IndexSettingAction.Invoke(indexSettings);
        else
        {
            string pinyinFilter = "pinyin";
            string wordDelimiterFilter = "word_delimiter";
            indexSettings.Analysis.Analyzers.Add(analyzer, new CustomAnalyzer()
            {
                Filter = new[] { pinyinFilter, wordDelimiterFilter },
                Tokenizer = "ik_max_word"
            });
            indexSettings.Analysis.TokenFilters.Add(pinyinFilter, new PinYinTokenFilterDescriptor());
        }

        TypeMappingDescriptor<TDocument> mapping = new TypeMappingDescriptor<TDocument>();
        if (option.Action != null)
            option.Action.Invoke(mapping);
        else
        {
            mapping = mapping
                .AutoMap<TDocument>()
                .Properties(ps =>
                    ps.Text(s =>
                        s.Name(n => n.Id)
                    )
                )
                .Properties(ps =>
                    ps.Text(s =>
                        s.Name(n => n.Text)
                            .Analyzer(analyzer)
                    )
                );
        }

        var createIndexResponse = client.CreateIndexAsync(indexName, new CreateIndexOptions()
        {
            Aliases = aliases,
            Mappings = mapping,
            IndexSettings = indexSettings
        }).ConfigureAwait(false).GetAwaiter().GetResult();
        if (!createIndexResponse.IsValid)
            logger?.LogError("AutoComplete: Initialization index is abnormal, {Message}", createIndexResponse.Message);
    }
}
