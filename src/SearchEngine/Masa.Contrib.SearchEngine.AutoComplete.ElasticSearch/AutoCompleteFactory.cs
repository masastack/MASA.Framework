// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.SearchEngine.AutoComplete;

public class AutoCompleteFactory : IAutoCompleteFactory
{
    private readonly List<AutoCompleteRelations> _relations;

    public AutoCompleteFactory(AutoCompleteRelationsOptions options) => _relations = options.Relations;

    public IAutoCompleteClient CreateClient()
    {
        var item = _relations.SingleOrDefault(r => r.IsDefault) ?? _relations.FirstOrDefault();
        ArgumentNullException.ThrowIfNull(item, "You should use AddAutoComplete before the project starts");
        return new AutoCompleteClient(
            item.ElasticClient,
            item.MasaElasticClient,
            item.RealIndexName,
            item.DefaultOperator,
            item.DefaultSearchType,
            item.EnableMultipleCondition);
    }

    /// <summary>
    /// Create a client corresponding to the index
    /// </summary>
    /// <param name="name">indexName or alias</param>
    /// <returns></returns>
    public IAutoCompleteClient CreateClient(string name)
    {
        var item = _relations.FirstOrDefault(relation => relation.IndexName == name || relation.Alias == name);
        ArgumentNullException.ThrowIfNull(item, nameof(name));
        return new AutoCompleteClient(
            item.ElasticClient,
            item.MasaElasticClient,
            item.RealIndexName,
            item.DefaultOperator,
            item.DefaultSearchType,
            item.EnableMultipleCondition);
    }
}
