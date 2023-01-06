// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    private static MasaElasticsearchBuilder CreateElasticsearchClient(
        this IServiceCollection services,
        string name,
        bool alwaysGetNewestElasticClient = false)
        => new(services, name, alwaysGetNewestElasticClient);

    public static MasaElasticsearchBuilder AddElasticsearchClient(this IServiceCollection services)
        => services.AddElasticsearch().CreateElasticsearchClient(Constant.DEFAULT_CLIENT_NAME);

    public static MasaElasticsearchBuilder AddElasticsearchClient(this IServiceCollection services, string[]? nodes)
        => services
            .AddElasticsearch(Constant.DEFAULT_CLIENT_NAME, nodes == null || nodes.Length == 0 ? new[] { "http://localhost:9200" } : nodes)
            .CreateElasticsearchClient(Constant.DEFAULT_CLIENT_NAME);

    public static MasaElasticsearchBuilder AddElasticsearchClient(this IServiceCollection services, string name, params string[] nodes)
        => services.AddElasticsearch(name, nodes).CreateElasticsearchClient(name);

    public static MasaElasticsearchBuilder AddElasticsearchClient(this IServiceCollection services,
        Action<ElasticsearchOptions> action,
        bool alwaysGetNewestElasticClient = false)
        => services.AddElasticsearchClient(Constant.DEFAULT_CLIENT_NAME, action, alwaysGetNewestElasticClient);

    public static MasaElasticsearchBuilder AddElasticsearchClient(this IServiceCollection services,
        string name,
        Action<ElasticsearchOptions> action,
        bool alwaysGetNewestElasticClient = false)
        => services.AddElasticsearch(name, action, alwaysGetNewestElasticClient)
            .CreateElasticsearchClient(name, alwaysGetNewestElasticClient);

    public static MasaElasticsearchBuilder AddElasticsearchClient(this IServiceCollection services,
        Func<ElasticsearchOptions> func,
        bool alwaysGetNewestElasticClient = false)
        => services.AddElasticsearchClient(Constant.DEFAULT_CLIENT_NAME, func, alwaysGetNewestElasticClient);

    public static MasaElasticsearchBuilder AddElasticsearchClient(this IServiceCollection services,
        string name,
        Func<ElasticsearchOptions> func,
        bool alwaysGetNewestElasticClient = false)
        => services.AddElasticsearch(name, func, alwaysGetNewestElasticClient)
            .CreateElasticsearchClient(name, alwaysGetNewestElasticClient);
}
