// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

internal static class ServiceCollectionExtensions
{
    public static void AddAutoCompleteCore<TDocument>(this IServiceCollection services,
        IElasticClient elasticClient,
        IMasaElasticClient client,
        AutoCompleteOptions<TDocument> option)
        where TDocument : AutoCompleteDocument
    {
        ArgumentNullException.ThrowIfNull(services);

        ArgumentNullException.ThrowIfNull(option.IndexName, nameof(option.IndexName));

        services.TryAddSingleton(new AutoCompleteRelationsOptions());

        var autoCompleteRelations = new AutoCompleteRelations(
            elasticClient,
            client,
            option.IndexName,
            option.Alias,
            option.IsDefault,
            option.DefaultOperator,
            option.DefaultSearchType,
            option.EnableMultipleCondition,
            option.QueryAnalyzer);
        services.TryAddAutoCompleteRelation(autoCompleteRelations);

        services.TryAddSingleton<IAutoCompleteFactory, AutoCompleteFactory>();
        services.TryAddSingleton(serviceProvider => serviceProvider.GetRequiredService<IAutoCompleteFactory>().CreateClient());
        client.TryCreateIndexAsync(services.BuildServiceProvider().GetService<ILogger<IAutoCompleteClient>>(), option);

        MasaApp.TrySetServiceCollection(services);
    }

#pragma warning disable S2326
    public static void TryAddAutoCompleteRelation(this IServiceCollection services, AutoCompleteRelations relation)
    {
        var serviceProvider = services.BuildServiceProvider();
        var relationsOptions = serviceProvider.GetRequiredService<AutoCompleteRelationsOptions>();

        if (relationsOptions.Relations.Any(r => r.Alias == relation.Alias || r.IndexName == relation.IndexName))
            throw new ArgumentException($"indexName or alias exists");

        if (relation.IsDefault && relationsOptions.Relations.Any(r => r.IsDefault))
            throw new ArgumentException("ElasticClient can only have one default", nameof(relation));

        relationsOptions.AddRelation(relation);
    }
#pragma warning disable S2326
}
