// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.SearchEngine.AutoComplete.ElasticSearch")]

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

internal static class AutoCompleteOptionsBuilderExtensions
{
    public static void AddAutoComplete(
        this AutoCompleteOptionsBuilder autoCompleteOptionsBuilder,
        Func<IServiceProvider, IManualAutoCompleteClient> implementationFactory)
    {
        MasaArgumentException.ThrowIfNull(implementationFactory);

        autoCompleteOptionsBuilder.Services.Configure<AutoCompleteFactoryOptions>(factoryOptions =>
        {
            if (factoryOptions.Options.Any(relation => relation.Name == autoCompleteOptionsBuilder.Name))
                throw new ArgumentException(
                    $"The {nameof(IAutoCompleteClient)} name already exists, please change the name, the repeat name is [{autoCompleteOptionsBuilder.Name}]");

            factoryOptions.Options.Add(new AutoCompleteRelationsOptions(autoCompleteOptionsBuilder.Name, implementationFactory));
        });
        autoCompleteOptionsBuilder.Services.AddAutoCompleteBySpecifyDocumentCore(autoCompleteOptionsBuilder.Name);
    }

    private static IServiceCollection AddAutoCompleteBySpecifyDocumentCore(
        this IServiceCollection services,
        string name)
    {
        MasaArgumentException.ThrowIfNull(services);
        MasaArgumentException.ThrowIfNull(name);

        services.TryAddTransient<IAutoCompleteFactory, DefaultAutoCompleteFactory>();
        services.TryAddSingleton(serviceProvider
            => (IAutoCompleteClient)serviceProvider.GetRequiredService<IAutoCompleteFactory>().Create());

        services.AddServiceFactory();
        MasaApp.TrySetServiceCollection(services);
        return services;
    }
}
