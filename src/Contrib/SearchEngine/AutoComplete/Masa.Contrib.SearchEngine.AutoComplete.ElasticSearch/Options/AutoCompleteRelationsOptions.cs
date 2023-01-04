// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.SearchEngine.AutoComplete.ElasticSearch;

// public class AutoCompleteRelationsOptions
// {
//     internal List<AutoCompleteRelations> Relations = new();
//
//     public AutoCompleteRelationsOptions AddRelation(AutoCompleteRelations options)
//     {
//         Relations.Add(options);
//         return this;
//     }
// }

public class AutoCompleteRelationsOptions : MasaRelationOptions<IAutoCompleteClient>
{
    internal bool IsDefault { get; }

    internal string IndexName { get; }

    internal string? Alias { get; }

    internal string RealIndexName { get; }

    internal Operator DefaultOperator { get; }

    internal bool EnableMultipleCondition { get; }

    internal string EsName { get; }

    internal bool IsSupportUpdate { get; }

    internal SearchType DefaultSearchType { get; }

    internal string QueryAnalyzer { get; }

    internal AutoCompleteRelationsOptions(
        string esName,
        Func<IServiceProvider, IAutoCompleteClient> func,
        bool isSupportUpdate,
        string indexName,
        string? alias,
        bool isDefault,
        Operator defaultOperator,
        SearchType defaultSearchType,
        bool enableMultipleCondition,
        string queryAnalyzer) : base(esName)
    {
        EsName = esName;
        Func = func;

        IsSupportUpdate = isSupportUpdate;
        IndexName = indexName;
        Alias = alias;
        RealIndexName = alias ?? indexName;
        IsDefault = isDefault;
        DefaultOperator = defaultOperator;
        DefaultSearchType = defaultSearchType;
        EnableMultipleCondition = enableMultipleCondition;
        QueryAnalyzer = queryAnalyzer;
    }
}
