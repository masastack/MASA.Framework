// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch;

public class ElasticsearchOptions
{
    public bool IsDefault { get; set; }

    public bool UseConnectionPool { get; private set; }

    internal string[] Nodes { get; private set; }

    internal StaticConnectionPoolOptions StaticConnectionPoolOptions { get; }

    internal ConnectionSettingsOptions ConnectionSettingsOptions { get; }

    internal Action<ConnectionSettings>? Action { get; private set; }

    public ElasticsearchOptions(params string[] nodes)
    {
        if (nodes.Length == 0)
            throw new ArgumentException("Please specify the Elasticsearch node address");

        IsDefault = false;
        Nodes = nodes;
        UseConnectionPool = nodes.Length > 1;
        ConnectionSettingsOptions = new();
        StaticConnectionPoolOptions = new();
        Action = null;
    }

    public ElasticsearchOptions UseDefault()
    {
        IsDefault = true;
        return this;
    }

    public ElasticsearchOptions UseNodes(params string[] nodes)
    {
        if (nodes == null || nodes.Length == 0)
            throw new ArgumentException("Please enter the Elasticsearch node address");

        Nodes = nodes;
        UseConnectionPool = nodes.Length > 1;
        return this;
    }

    public ElasticsearchOptions UseRandomize(bool randomize)
    {
        StaticConnectionPoolOptions.UseRandomize(randomize);
        return this;
    }

    public ElasticsearchOptions UseDateTimeProvider(IDateTimeProvider dateTimeProvider)
    {
        StaticConnectionPoolOptions.UseDateTimeProvider(dateTimeProvider);
        return this;
    }

    public ElasticsearchOptions UseConnectionSettings(Action<ConnectionSettings> action)
    {
        Action = action;
        return this;
    }
}
