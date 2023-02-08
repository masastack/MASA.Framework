// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Utils.Data.Elasticsearch;

public class ElasticsearchOptions
{
    private bool? _useConnectionPool;

    public bool UseConnectionPool
    {
        get => (_useConnectionPool == null && Nodes.Count() > 1) || _useConnectionPool == true;
        set => _useConnectionPool = value;
    }

    private IEnumerable<string> _nodes;

    public IEnumerable<string> Nodes
    {
        get => _nodes;
        set
        {
            if (value == null || !value.Any())
                throw new ArgumentException("Please enter the Elasticsearch node address");

            _nodes = value;
        }
    }

    public StaticConnectionPoolOptions StaticConnectionPoolOptions { get; }

    public ConnectionSettingsOptions ConnectionSettingsOptions { get; }

    public Action<ConnectionSettings>? Action { get; set; }

    public ElasticsearchOptions()
    {
        ConnectionSettingsOptions = new();
        StaticConnectionPoolOptions = new();
        Action = null;
    }

    public ElasticsearchOptions(params string[] nodes) : this() => Nodes = nodes;

    public ElasticsearchOptions UseNodes(params string[] nodes)
    {
        Nodes = nodes;
        return this;
    }

    public ElasticsearchOptions UseRandomize(bool randomize)
    {
        StaticConnectionPoolOptions.Randomize = randomize;
        return this;
    }

    public ElasticsearchOptions UseDateTimeProvider(IDateTimeProvider? dateTimeProvider)
    {
        StaticConnectionPoolOptions.DateTimeProvider = dateTimeProvider;
        return this;
    }

    public ElasticsearchOptions UseConnectionSettings(Action<ConnectionSettings>? action)
    {
        Action = action;
        return this;
    }
}
