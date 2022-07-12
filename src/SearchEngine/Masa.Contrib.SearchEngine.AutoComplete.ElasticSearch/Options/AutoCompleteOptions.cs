// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.SearchEngine.AutoComplete.Options;

public class AutoCompleteOptions<TDocument, TValue>
    where TDocument : AutoCompleteDocument<TValue> where TValue : notnull
{
    internal string IndexName { get; private set; }

    internal string? Alias { get; private set; }

    internal bool IsDefault { get; private set; }

    internal SearchType DefaultSearchType { get; private set; } = SearchType.Fuzzy;

    internal Operator DefaultOperator { get; private set; } = Operator.Or;

    internal Action<TypeMappingDescriptor<TDocument>>? Action { get; private set; }

    internal Action<IIndexSettings>? IndexSettingAction { get; private set; }

    public AutoCompleteOptions<TDocument, TValue> UseIndexName(string indexName)
    {
        IndexName = indexName;
        return this;
    }

    /// <summary>
    /// Set index alias
    /// </summary>
    /// <param name="alias">When it is null, no alias is set</param>
    /// <returns></returns>
    public AutoCompleteOptions<TDocument, TValue> UseAlias(string alias)
    {
        Alias = alias;
        return this;
    }

    /// <summary>
    /// Set the default AutoComplete
    /// </summary>
    /// <returns></returns>
    public AutoCompleteOptions<TDocument, TValue> UseDefault()
    {
        IsDefault = true;
        return this;
    }

    public AutoCompleteOptions<TDocument, TValue> UseDefaultSearchType(SearchType defaultSearchType)
    {
        DefaultSearchType = defaultSearchType;
        return this;
    }

    /// <summary>
    /// custom mapping
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public AutoCompleteOptions<TDocument, TValue> Mapping(Action<TypeMappingDescriptor<TDocument>> action)
    {
        Action = action;
        return this;
    }

    public AutoCompleteOptions<TDocument, TValue> IndexSettings(Action<IIndexSettings> indexSettingAction)
    {
        IndexSettingAction = indexSettingAction;
        return this;
    }

    public AutoCompleteOptions<TDocument, TValue> UseDefaultOperator(Operator defaultOperator)
    {
        DefaultOperator = defaultOperator;
        return this;
    }

}
