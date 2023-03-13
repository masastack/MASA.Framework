// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAutoComplete(this IServiceCollection services, Action<AutoCompleteOptionsBuilder> configure)
        => services.AddAutoComplete(Microsoft.Extensions.Options.Options.DefaultName, configure);

    public static IServiceCollection AddAutoComplete(this IServiceCollection services, string name,
        Action<AutoCompleteOptionsBuilder> configure)
        => services.AddAutoComplete<Guid>(name, configure);

    public static IServiceCollection AddAutoComplete(
        this IServiceCollection services,
        string name,
        Func<IServiceProvider, IAutoCompleteClient> implementationFactory)
    {
        MasaArgumentException.ThrowIfNull(implementationFactory);

        services.Configure<AutoCompleteFactoryOptions>(factoryOptions =>
        {
            if (factoryOptions.Options.Any(relation => relation.Name == name))
                throw new ArgumentException($"The {nameof(IAutoCompleteClient)} name already exists, please change the name, the repeat name is [{name}]");

            factoryOptions.Options.Add(new AutoCompleteRelationsOptions(name, implementationFactory));
        });
        return services.AddAutoCompleteBySpecifyDocumentCore(name);
    }

    public static IServiceCollection AddAutoComplete<TValue>(
        this IServiceCollection services,
        Action<AutoCompleteOptionsBuilder> configure)
        where TValue : notnull
        => services.AddAutoCompleteBySpecifyDocument<AutoCompleteDocument<TValue>>(
            Microsoft.Extensions.Options.Options.DefaultName,
            configure);

    public static IServiceCollection AddAutoComplete<TValue>(
        this IServiceCollection services,
        string name,
        Action<AutoCompleteOptionsBuilder> configure)
        where TValue : notnull
        => services.AddAutoCompleteBySpecifyDocument<AutoCompleteDocument<TValue>>(name, configure);

    public static IServiceCollection AddAutoCompleteBySpecifyDocument<TDocument>(
        this IServiceCollection services,
        Action<AutoCompleteOptionsBuilder> configure)
        where TDocument : AutoCompleteDocument
        => services.AddAutoCompleteBySpecifyDocument<TDocument>(Options.Options.DefaultName, configure);

    public static IServiceCollection AddAutoCompleteBySpecifyDocument<TDocument>(
        this IServiceCollection services,
        string name,
        Action<AutoCompleteOptionsBuilder> configure)
        where TDocument : AutoCompleteDocument
    {
        MasaArgumentException.ThrowIfNull(configure);
        configure.Invoke(new AutoCompleteOptionsBuilder(services, name, typeof(TDocument)));
        return services;
    }

    private static IServiceCollection AddAutoCompleteBySpecifyDocumentCore(
        this IServiceCollection services,
        string name)
    {
        MasaArgumentException.ThrowIfNull(services);
        MasaArgumentException.ThrowIfNull(name);

        services.TryAddSingleton<IAutoCompleteFactory, AutoCompleteFactory>();
        services.TryAddSingleton(serviceProvider => serviceProvider.GetRequiredService<IAutoCompleteFactory>().Create());

        services.AddServiceFactory();
        MasaApp.TrySetServiceCollection(services);
        return services;
    }
}
