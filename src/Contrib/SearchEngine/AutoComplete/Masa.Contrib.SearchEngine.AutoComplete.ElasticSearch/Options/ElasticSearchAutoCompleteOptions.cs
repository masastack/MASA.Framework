// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.SearchEngine.AutoComplete.ElasticSearch;

public class ElasticSearchAutoCompleteOptions
{
    public ElasticsearchOptions ElasticsearchOptions { get; set; }

    public string IndexName { get; set; }

    public string? Alias { get; set; }

    public SearchType DefaultSearchType { get; set; } = SearchType.Fuzzy;

    public Operator DefaultOperator { get; set; } = Operator.Or;

    public bool EnableMultipleCondition { get; set; } = true;

    public Action<ITypeMapping>? Action { get; set; }

    public Action<IIndexSettings>? IndexSettingAction { get; set; }

    public string QueryAnalyzer { get; set; } = "ik_smart";

    public ElasticSearchAutoCompleteOptions() => ElasticsearchOptions = new();

    public Action<TypeMappingDescriptor<TDocument>> Mapping<TDocument>(Action<TypeMappingDescriptor<TDocument>> action)
        where TDocument : AutoCompleteDocument
    {
        Action = typeMapping =>
        {
            action.Invoke((TypeMappingDescriptor<TDocument>)typeMapping);
        };
        return action;
    }
}
