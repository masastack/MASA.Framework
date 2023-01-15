// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    private static MasaElasticsearchBuilder CreateElasticsearchClient(this IServiceCollection services, string name)
        => new(services, name);

    public static MasaElasticsearchBuilder AddElasticsearchClient(this IServiceCollection services)
        => services.AddElasticsearch(Constant.DEFAULT_CLIENT_NAME).CreateElasticsearchClient(Constant.DEFAULT_CLIENT_NAME);

    public static MasaElasticsearchBuilder AddElasticsearchClient(this IServiceCollection services, IEnumerable<string> nodes)
        => services.AddElasticsearch(Constant.DEFAULT_CLIENT_NAME, nodes).CreateElasticsearchClient(Constant.DEFAULT_CLIENT_NAME);

    public static MasaElasticsearchBuilder AddElasticsearchClient(this IServiceCollection services, Action<ElasticsearchOptions> action)
        => services.AddElasticsearchClient(Constant.DEFAULT_CLIENT_NAME, action);

    public static MasaElasticsearchBuilder AddElasticsearchClient(this IServiceCollection services, string name, params string[] nodes)
        => services.AddElasticsearch(name, nodes).CreateElasticsearchClient(name);

    public static MasaElasticsearchBuilder AddElasticsearchClient(this IServiceCollection services, string name, IEnumerable<string> nodes)
        => services.AddElasticsearch(name, nodes).CreateElasticsearchClient(name);

    public static MasaElasticsearchBuilder AddElasticsearchClient(this IServiceCollection services,
        string name,
        Action<ElasticsearchOptions> action)
        => services.AddElasticsearch(name, action).CreateElasticsearchClient(name);
}
