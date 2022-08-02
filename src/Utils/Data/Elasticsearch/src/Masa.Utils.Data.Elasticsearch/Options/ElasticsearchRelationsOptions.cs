// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options;

public class ElasticsearchRelationsOptions
{
    internal readonly Dictionary<string, ElasticsearchRelations> Relations = new();

    public ElasticsearchRelationsOptions AddRelation(string name, ElasticsearchOptions options)
    {
        Uri[] nodes = options.Nodes.Select(uriString => new Uri(uriString)).ToArray();
        ElasticsearchRelations relation = new ElasticsearchRelations(name, options.UseConnectionPool, nodes)
            .UseStaticConnectionPoolOptions(options.StaticConnectionPoolOptions)
            .UseConnectionSettingsOptions(options.ConnectionSettingsOptions)
            .UseConnectionSettings(options.Action);
        Relations.Add(name, relation);
        return this;
    }
}
