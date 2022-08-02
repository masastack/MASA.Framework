// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch;

public class ElasticsearchRelations
{
    public bool IsDefault { get; private set; }

    public string Name { get; }

    public Uri[] Nodes { get; }

    public bool UseConnectionPool { get; }

    internal StaticConnectionPoolOptions? StaticConnectionPoolOptions { get; private set; }

    internal ConnectionSettingsOptions? ConnectionSettingsOptions { get; private set; }

    internal Action<ConnectionSettings>? Action { get; private set; }

    public ElasticsearchRelations(string name, bool useConnectionPool, Uri[] nodes)
    {
        Name = name;
        IsDefault = false;
        UseConnectionPool = useConnectionPool;
        Nodes = nodes;
        Action = null;
        StaticConnectionPoolOptions = null;
        ConnectionSettingsOptions = null;
    }

    public ElasticsearchRelations UseDefault()
    {
        IsDefault = true;
        return this;
    }

    public ElasticsearchRelations UseStaticConnectionPoolOptions(StaticConnectionPoolOptions staticConnectionPoolOptions)
    {
        StaticConnectionPoolOptions = staticConnectionPoolOptions;
        return this;
    }

    public ElasticsearchRelations UseConnectionSettingsOptions(ConnectionSettingsOptions connectionSettingsOptions)
    {
        ConnectionSettingsOptions = connectionSettingsOptions;
        return this;
    }

    public ElasticsearchRelations UseConnectionSettings(Action<ConnectionSettings>? action)
    {
        Action = action;
        return this;
    }
}
