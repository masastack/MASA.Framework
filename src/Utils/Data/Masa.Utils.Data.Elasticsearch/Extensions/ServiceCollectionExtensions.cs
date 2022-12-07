// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddElasticsearch(this IServiceCollection services, string[]? nodes = null)
    {
        if (nodes == null || nodes.Length == 0)
        {
            nodes = new[] { "http://localhost:9200" };
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
        ArgumentNullException.ThrowIfNull(name);

        services
            .AddElasticsearchCore()
            .AddElasticsearchOptions(name, func.Invoke());

        return services;
    }

    private static IServiceCollection AddElasticsearchCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<IElasticsearchFactory, DefaultElasticsearchFactory>();

        services.TryAddSingleton(serviceProvider =>
            serviceProvider.GetRequiredService<IElasticClientFactory>().Create());

        services.TryAddSingleton(serviceProvider
            => serviceProvider.GetRequiredService<IMasaElasticClientFactory>().Create());

        services.TryAddSingleton<IElasticClientFactory, DefaultElasticClientFactory>();
        services.TryAddSingleton<IMasaElasticClientFactory, DefaultMasaElasticClientFactory>();

        return services;
    }

    private static void AddElasticsearchOptions(
        this IServiceCollection services,
        string name,
        ElasticsearchOptions elasticsearchOptions)
    {
        services.Configure<ElasticsearchOptions>(name, options =>
        {
            if (elasticsearchOptions.IsDefault) options.UseDefault();

            options.UseNodes(elasticsearchOptions.Nodes);
            options.UseRandomize(elasticsearchOptions.StaticConnectionPoolOptions.Randomize);
            options.UseDateTimeProvider(elasticsearchOptions.StaticConnectionPoolOptions.DateTimeProvider);
            options.ConnectionSettingsOptions.UseConnection(elasticsearchOptions.ConnectionSettingsOptions.Connection);
            options.ConnectionSettingsOptions.UseSourceSerializerFactory(elasticsearchOptions.ConnectionSettingsOptions
                .SourceSerializerFactory);
            options.ConnectionSettingsOptions.UsePropertyMappingProvider(elasticsearchOptions.ConnectionSettingsOptions
                .PropertyMappingProvider);
            options.UseConnectionSettings(elasticsearchOptions.Action);
        });
    }
}
