// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    internal static void AddAutoCompleteCore<TDocument>(this IServiceCollection services,
        string esName,
        Action<AutoCompleteOptions<TDocument>> action)
        where TDocument : AutoCompleteDocument
    {
        ArgumentNullException.ThrowIfNull(services);

        
        // ArgumentNullException.ThrowIfNull(option.IndexName, nameof(option.IndexName));
        //
        // services.TryOrUpdate()
        //
        // services.TryAddSingleton(new AutoCompleteRelationsOptions());
        //
        // var autoCompleteRelations = new AutoCompleteRelationsOptions(
        //     esName,
        //     option.IndexName,
        //     option.Alias,
        //     option.IsDefault,
        //     option.DefaultOperator,
        //     option.DefaultSearchType,
        //     option.EnableMultipleCondition,
        //     option.QueryAnalyzer);
        // services.TryAddAutoCompleteRelation(autoCompleteRelations);

        services.TryAddSingleton<IAutoCompleteFactory, AutoCompleteFactory>();
        services.TryAddSingleton(serviceProvider => serviceProvider.GetRequiredService<IAutoCompleteFactory>().Create());
        // client.TryCreateIndexAsync(services.BuildServiceProvider().GetService<ILogger<IAutoCompleteClient>>(), option);

        MasaApp.TrySetServiceCollection(services);
    }

    // private static void TryAddAutoCompleteRelation(this IServiceCollection services, AutoCompleteRelationsOptions relation)
    // {
    //     var serviceProvider = services.BuildServiceProvider();
    //     var relationsOptions = serviceProvider.GetRequiredService<AutoCompleteRelationsOptions>();
    //
    //     if (relationsOptions.Relations.Any(r => r.Alias == relation.Alias || r.IndexName == relation.IndexName))
    //         throw new ArgumentException($"indexName or alias exists");
    //
    //     if (relation.IsDefault && relationsOptions.Relations.Any(r => r.IsDefault))
    //         throw new ArgumentException("ElasticClient can only have one default", nameof(ElasticsearchOptions.IsDefault));
    //
    //     relationsOptions.AddRelation(relation);
    // }

    private static void TryCreateIndexAsync<TDocument>(
        this IMasaElasticClient client,
        ILogger<IAutoCompleteClient>? logger,
        AutoCompleteOptions<TDocument> option)
        where TDocument : AutoCompleteDocument
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

    private static void CreateIndex<TDocument>(
        this IMasaElasticClient client,
        ILogger<IAutoCompleteClient>? logger,
        string indexName,
        IAliases? aliases,
        AutoCompleteOptions<TDocument> option)
        where TDocument : AutoCompleteDocument
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
            indexSettings.Analysis.Analyzers.Add(analyzer, new CustomAnalyzer()
            {
                Filter = new[] { pinyinFilter, "lowercase" },
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

    private static IServiceCollection TryOrUpdate(
        this IServiceCollection services,
        AutoCompleteRelationsOptions options)
    {
        services.Configure<AutoCompleteFactoryOptions>(factoryOptions =>
        {
            if (factoryOptions.Options.Any(relation => relation.Name == options.Name))
                throw new ArgumentException($"The caller name already exists, please change the name, the repeat name is [{options.Name}]");

            factoryOptions.Options.Add(options);
        });
        return services;
    }
}
