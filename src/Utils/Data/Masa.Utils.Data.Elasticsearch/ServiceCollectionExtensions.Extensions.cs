// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    private static MasaElasticsearchBuilder CreateElasticsearchClient(this IServiceCollection services, string name)
    {
        var elasticClient = services.BuildServiceProvider().GetRequiredService<IElasticsearchFactory>().CreateElasticClient(name);
        return new MasaElasticsearchBuilder(services, elasticClient);
    }

    public static MasaElasticsearchBuilder AddElasticsearchClient(this IServiceCollection services)
        => services.AddElasticsearch().CreateElasticsearchClient(Const.DEFAULT_CLIENT_NAME);

    public static MasaElasticsearchBuilder AddElasticsearchClient(this IServiceCollection services, string[]? nodes)
        => services
            .AddElasticsearch(Const.DEFAULT_CLIENT_NAME, nodes == null || nodes.Length == 0 ? new[] {"http://localhost:9200"} : nodes)
            .CreateElasticsearchClient(Const.DEFAULT_CLIENT_NAME);

    public static MasaElasticsearchBuilder AddElasticsearchClient(this IServiceCollection services, string name, params string[] nodes)
        => services.AddElasticsearch(name, nodes).CreateElasticsearchClient(name);

    public static MasaElasticsearchBuilder AddElasticsearchClient(this IServiceCollection services, string name, Action<ElasticsearchOptions> action)
        => services.AddElasticsearch(name, action).CreateElasticsearchClient(name);

    public static MasaElasticsearchBuilder AddElasticsearchClient(this IServiceCollection services, string name, Func<ElasticsearchOptions> func)
        => services.AddElasticsearch(name, func).CreateElasticsearchClient(name);
}
