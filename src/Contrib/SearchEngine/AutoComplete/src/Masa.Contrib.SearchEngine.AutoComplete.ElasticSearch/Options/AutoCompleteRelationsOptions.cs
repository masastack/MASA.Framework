﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.SearchEngine.AutoComplete.Options;

public class AutoCompleteRelationsOptions
{
    internal List<AutoCompleteRelations> Relations = new();

    public AutoCompleteRelationsOptions AddRelation(AutoCompleteRelations options)
    {
        Relations.Add(options);
        return this;
    }
}

public class AutoCompleteRelations
{
    internal bool IsDefault { get; }

    internal string IndexName { get; }

    internal string? Alias { get; }

    internal string RealIndexName { get; }

    internal Operator DefaultOperator { get; }

    internal bool EnableMultipleCondition { get; }

    internal IElasticClient ElasticClient { get; }

    internal IMasaElasticClient MasaElasticClient { get; }

    internal SearchType DefaultSearchType { get; }

    internal AutoCompleteRelations(
        IElasticClient elasticClient,
        IMasaElasticClient masaElasticClient,
        string indexName,
        string? alias,
        bool isDefault,
        Operator defaultOperator,
        SearchType defaultSearchType,
        bool enableMultipleCondition)
    {
        ElasticClient = elasticClient;
        MasaElasticClient = masaElasticClient;
        IndexName = indexName;
        Alias = alias;
        RealIndexName = alias ?? indexName;
        IsDefault = isDefault;
        DefaultOperator = defaultOperator;
        DefaultSearchType = defaultSearchType;
        EnableMultipleCondition = enableMultipleCondition;
    }
}
