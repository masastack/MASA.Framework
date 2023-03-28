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

        services.TryAddTransient<IAutoCompleteFactory, DefaultAutoCompleteFactory>();
        services.TryAddSingleton<SingletonService<IAutoCompleteClient>>(serviceProvider
            => new SingletonService<IAutoCompleteClient>(serviceProvider.GetRequiredService<IAutoCompleteFactory>().Create()));
        services.TryAddScoped<ScopedService<IAutoCompleteClient>>(serviceProvider
            => new ScopedService<IAutoCompleteClient>(serviceProvider.GetRequiredService<IAutoCompleteFactory>().Create()));

        services.TryAddTransient<IManualAutoCompleteClient>(serviceProvider =>
        {
            var autoCompleteClient = serviceProvider.EnableIsolation() ?
                serviceProvider.GetRequiredService<ScopedService<IManualAutoCompleteClient>>().Service :
                serviceProvider.GetRequiredService<SingletonService<IManualAutoCompleteClient>>().Service;
            return new DefaultIAutoCompleteClient(autoCompleteClient);
        });
        services.TryAddTransient<IAutoCompleteClient>(serviceProvider => serviceProvider.GetRequiredService<IManualAutoCompleteClient>());

        MasaApp.TrySetServiceCollection(services);

        configure.Invoke(new AutoCompleteOptionsBuilder(services, name, typeof(TDocument)));
        return services;
    }
}
