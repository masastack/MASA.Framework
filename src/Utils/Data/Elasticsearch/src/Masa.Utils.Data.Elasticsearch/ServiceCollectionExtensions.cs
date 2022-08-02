// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddElasticsearch(this IServiceCollection services, string[]? nodes = null)
    {
        if (nodes == null || nodes.Length == 0)
        {
            nodes = new[] {"http://localhost:9200"};
        }

        return services.AddElasticsearch(Const.DEFAULT_CLIENT_NAME, nodes);
    }

    public static IServiceCollection AddElasticsearch(this IServiceCollection services, string name, params string[] nodes)
        => services.AddElasticsearch(name, options => options.UseNodes(nodes));

    public static IServiceCollection AddElasticsearch(this IServiceCollection services, string name, Action<ElasticsearchOptions> action)
    {
        return services.AddElasticsearch(name, () =>
        {
            ElasticsearchOptions options = new("http://localhost:9200");
            action.Invoke(options);
            return options;
        });
    }

    public static IServiceCollection AddElasticsearch(this IServiceCollection services, string name, Func<ElasticsearchOptions> func)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        AddElasticsearchCore(services);

        services.TryAddOrUpdateElasticsearchRelation(name, func.Invoke());

        return services;
    }

    private static IServiceCollection AddElasticsearchCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<IElasticsearchFactory, DefaultElasticsearchFactory>();

        services.TryAddSingleton(serviceProvider =>
            serviceProvider.GetRequiredService<IElasticsearchFactory>().CreateElasticClient());

        services.TryAddSingleton<IMasaElasticClient>(serviceProvider =>
            new DefaultMasaElasticClient(serviceProvider.GetRequiredService<IElasticClient>()));

        services.TryAddSingleton(new ElasticsearchRelationsOptions());

        return services;
    }

    private static void TryAddOrUpdateElasticsearchRelation(this IServiceCollection services, string name, ElasticsearchOptions options)
    {
        var serviceProvider = services.BuildServiceProvider();
        var relationsOptions = serviceProvider.GetRequiredService<ElasticsearchRelationsOptions>();

        if (relationsOptions.Relations.ContainsKey(name))
            throw new ArgumentException($"The ElasticClient whose name is {name} is exist");

        if (options.IsDefault && relationsOptions.Relations.Values.Any(r => r.IsDefault))
            throw new ArgumentNullException(nameof(ElasticsearchRelations.IsDefault), "ElasticClient can only have one default");

        relationsOptions.AddRelation(name, options);
    }
}
