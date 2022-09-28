// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.Contrib.SearchEngine.AutoComplete.ElasticSearch;

public class AutoCompleteOptions<TDocument>
    where TDocument : AutoCompleteDocument
{
    internal string IndexName { get; private set; }

    internal string? Alias { get; private set; }

    internal bool IsDefault { get; private set; }

    internal SearchType DefaultSearchType { get; private set; } = SearchType.Fuzzy;

    internal Operator DefaultOperator { get; private set; } = Operator.Or;

    internal bool EnableMultipleCondition { get; private set; } = true;

    internal Action<TypeMappingDescriptor<TDocument>>? Action { get; private set; }

    internal Action<IIndexSettings>? IndexSettingAction { get; private set; }

    internal string QueryAnalyzer { get; private set; } = "ik_smart";

    public AutoCompleteOptions<TDocument> UseIndexName(string indexName)
    {
        IndexName = indexName;
        return this;
    }

    /// <summary>
    /// Set index alias
    /// </summary>
    /// <param name="alias">When it is null, no alias is set</param>
    /// <returns></returns>
    public AutoCompleteOptions<TDocument> UseAlias(string alias)
    {
        Alias = alias;
        return this;
    }

    /// <summary>
    /// Set the default AutoComplete
    /// </summary>
    /// <returns></returns>
    public AutoCompleteOptions<TDocument> UseDefault()
    {
        IsDefault = true;
        return this;
    }

    public AutoCompleteOptions<TDocument> UseDefaultSearchType(SearchType defaultSearchType)
    {
        DefaultSearchType = defaultSearchType;
        return this;
    }

    /// <summary>
    /// custom mapping
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public AutoCompleteOptions<TDocument> Mapping(Action<TypeMappingDescriptor<TDocument>> action)
    {
        Action = action;
        return this;
    }

    public AutoCompleteOptions<TDocument> IndexSettings(Action<IIndexSettings> indexSettingAction)
    {
        IndexSettingAction = indexSettingAction;
        return this;
    }

    public AutoCompleteOptions<TDocument> UseDefaultOperator(Operator defaultOperator)
    {
        DefaultOperator = defaultOperator;
        return this;
    }

    public AutoCompleteOptions<TDocument> UseMultipleConditions(bool enableMultipleCondition = true)
    {
        EnableMultipleCondition = enableMultipleCondition;
        return this;
    }

    public AutoCompleteOptions<TDocument> UseQueryAnalyzer(string queryAnalyzer)
    {
        QueryAnalyzer = queryAnalyzer;
        return this;
    }
}
