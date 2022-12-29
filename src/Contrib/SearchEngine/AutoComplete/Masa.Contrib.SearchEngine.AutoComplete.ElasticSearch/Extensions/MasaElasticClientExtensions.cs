// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete;

internal static class MasaElasticClientExtensions
{
    public static void TryCreateIndexAsync<TDocument>(
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

    public static void CreateIndex<TDocument>(
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
}
