// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddElasticsearch(this IServiceCollection services, params string[] nodes)
        => services.AddElasticsearch(nodes.AsEnumerable());

    public static IServiceCollection AddElasticsearch(this IServiceCollection services, IEnumerable<string> nodes)
        => services.AddElasticsearch(Constant.DEFAULT_CLIENT_NAME, nodes);

    public static IServiceCollection AddElasticsearch(this IServiceCollection services, string name, params string[] nodes)
        => services.AddElasticsearch(name, nodes.AsEnumerable());

    public static IServiceCollection AddElasticsearch(this IServiceCollection services, string name, IEnumerable<string> nodes)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(nodes);

        return services.AddElasticsearch(name, options =>
        {
            options.Nodes = !nodes.Any() ? new[] { "http://localhost:9200" } : nodes;
        });
    }

    public static IServiceCollection AddElasticsearch(this IServiceCollection services, Action<ElasticsearchOptions> action)
        => services.AddElasticsearch(Constant.DEFAULT_CLIENT_NAME, action);

    public static IServiceCollection AddElasticsearch(this IServiceCollection services,
        string name,
        Action<ElasticsearchOptions> action)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(action);

        return services
            .AddElasticsearchCore()
            .AddElasticsearchOptions(name, options =>
            {
                options.Func = _ =>
                {
                    var elasticsearchOptions = new ElasticsearchOptions();
                    action.Invoke(elasticsearchOptions);

                    return ElasticClientUtils.Create(elasticsearchOptions);
                };
            });
    }

    private static IServiceCollection AddElasticsearchCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<IElasticsearchFactory, DefaultElasticsearchFactory>();
        services.TryAddSingleton(serviceProvider => serviceProvider.GetRequiredService<IElasticClientFactory>().Create());
        services.TryAddSingleton(serviceProvider => serviceProvider.GetRequiredService<IMasaElasticClientFactory>().Create());

        services.TryAddSingleton<IElasticClientFactory, DefaultElasticClientFactory>();
        services.TryAddSingleton<IMasaElasticClientFactory, DefaultMasaElasticClientFactory>();

        return services;
    }

    private static IServiceCollection AddElasticsearchOptions(
        this IServiceCollection services,
        string name,
        Action<ElasticsearchRelationsOptions> action)
    {
        services.Configure<ElasticsearchFactoryOptions>(options =>
        {
            if (options.Options.Any(o => o.Name == name))
                throw new ArgumentException($"The es name already exists, please change the name, the repeat name is [{name}]");

            var relationsOptions = new ElasticsearchRelationsOptions(name);
            action.Invoke(relationsOptions);
            options.Options.Add(relationsOptions);
        });
        return services;
    }

}
