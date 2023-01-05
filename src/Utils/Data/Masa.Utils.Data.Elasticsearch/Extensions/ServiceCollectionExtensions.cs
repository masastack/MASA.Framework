// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddElasticsearch(this IServiceCollection services, params string[] nodes)
        => services.AddElasticsearch(Constant.DEFAULT_CLIENT_NAME, nodes);

    public static IServiceCollection AddElasticsearch(this IServiceCollection services, string name, params string[] nodes)
        => services.AddElasticsearch(name, options =>
        {
            if (nodes == null! || nodes.Length == 0) nodes = new[] { "http://localhost:9200" };

            options.UseNodes(nodes);
        });

    public static IServiceCollection AddElasticsearch(this IServiceCollection services,
        string name,
        Action<ElasticsearchOptions> action,
        bool alwaysGetNewestElasticClient = false)
    {
        ArgumentNullException.ThrowIfNull(name);
        return services
            .AddElasticsearchCore(alwaysGetNewestElasticClient ? ServiceLifetime.Scoped : ServiceLifetime.Singleton)
            .AddElasticsearchOptions(name, options =>
            {
                ConnectionSettings? settings = null;
                if (!alwaysGetNewestElasticClient)
                {
                    var elasticsearchOptions = new ElasticsearchOptions();
                    action.Invoke(elasticsearchOptions);

                    settings = elasticsearchOptions.UseConnectionPool
                        ? GetConnectionSettingsConnectionPool(elasticsearchOptions)
                        : GetConnectionSettingsBySingleNode(elasticsearchOptions);
                }
                options.Func = _ =>
                {
                    if (alwaysGetNewestElasticClient)
                    {
                        var elasticsearchOptions = new ElasticsearchOptions();
                        action.Invoke(elasticsearchOptions);

                        settings = elasticsearchOptions.UseConnectionPool
                            ? GetConnectionSettingsConnectionPool(elasticsearchOptions)
                            : GetConnectionSettingsBySingleNode(elasticsearchOptions);
                    }
                    return new ElasticClient(settings);
                };
            });
    }

    public static IServiceCollection AddElasticsearch(this IServiceCollection services,
        string name,
        Func<ElasticsearchOptions> func,
        bool alwaysGetNewestElasticClient)
    {
        return services.AddElasticsearch(name, options =>
        {
            var elasticsearchOptions = func.Invoke();
            options.UseNodes(elasticsearchOptions.Nodes);
            options.UseRandomize(elasticsearchOptions.StaticConnectionPoolOptions.Randomize);
            options.UseDateTimeProvider(elasticsearchOptions.StaticConnectionPoolOptions.DateTimeProvider);
            options.ConnectionSettingsOptions.UseConnection(elasticsearchOptions.ConnectionSettingsOptions.Connection);
            options.ConnectionSettingsOptions.UseSourceSerializerFactory(elasticsearchOptions.ConnectionSettingsOptions
                .SourceSerializerFactory);
            options.ConnectionSettingsOptions.UsePropertyMappingProvider(elasticsearchOptions.ConnectionSettingsOptions
                .PropertyMappingProvider);
            options.UseConnectionSettings(elasticsearchOptions.Action);
        }, alwaysGetNewestElasticClient);
    }

    private static IServiceCollection AddElasticsearchCore(this IServiceCollection services, ServiceLifetime serviceLifetime)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<IElasticsearchFactory, DefaultElasticsearchFactory>();

        var elasticClientServiceDescriptor = new ServiceDescriptor(
            typeof(IElasticClient),
            serviceProvider => serviceProvider.GetRequiredService<IElasticClientFactory>().Create(),
            serviceLifetime
        );
        services.TryAdd(elasticClientServiceDescriptor);

        var masaElasticClientServiceDescriptor = new ServiceDescriptor(
            typeof(IMasaElasticClient),
            serviceProvider => serviceProvider.GetRequiredService<IMasaElasticClientFactory>().Create(),
            serviceLifetime
        );
        services.TryAdd(masaElasticClientServiceDescriptor);

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

    private static ConnectionSettings GetConnectionSettingsBySingleNode(ElasticsearchOptions relation)
    {
        ArgumentNullException.ThrowIfNull(relation.Nodes);

        var connectionSetting = new ConnectionSettings(new Uri(relation.Nodes[0]))
            .EnableApiVersioningHeader();
        relation.Action?.Invoke(connectionSetting);
        return connectionSetting;
    }

    private static ConnectionSettings GetConnectionSettingsConnectionPool(ElasticsearchOptions relation)
    {
        var pool = new StaticConnectionPool(
            relation.Nodes.Select(node => new Uri(node)),
            relation.StaticConnectionPoolOptions?.Randomize ?? true,
            relation.StaticConnectionPoolOptions?.DateTimeProvider);

        var settings = new ConnectionSettings(
                pool,
                relation.ConnectionSettingsOptions.Connection,
                relation.ConnectionSettingsOptions.SourceSerializerFactory,
                relation.ConnectionSettingsOptions.PropertyMappingProvider)
            .EnableApiVersioningHeader();

        relation.Action?.Invoke(settings);
        return settings;
    }
}
