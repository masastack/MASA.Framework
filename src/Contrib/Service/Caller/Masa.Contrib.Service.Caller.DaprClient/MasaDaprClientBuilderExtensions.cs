// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.DaprClient;

public static class MasaDaprClientBuilderExtensions
{
    public static DefaultDaprClientBuilder AddHttpRequestMessage<TRequestMessage>(
        this DefaultDaprClientBuilder builder)
        where TRequestMessage : class, IDaprRequestMessage
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.TryAddEnumerable(new ServiceDescriptor(
            typeof(IDaprRequestMessage),
            typeof(TRequestMessage),
            ServiceLifetime.Singleton));

        builder.Services.Configure<CallerDaprClientOptions>(builder.Name, option =>
        {
            option.HttpRequestMessageActions.Add(b
                => b.RequestMessages.Add(
                    b.ServiceProvider
                        .GetServices<IDaprRequestMessage>()
                        .FirstOrDefault(d => d.GetType() == typeof(TRequestMessage))!));
        });
        return builder;
    }

    public static IHttpClientBuilder AddHttpRequestMessage<TRequestMessage>(
        this IHttpClientBuilder builder,
        Func<IServiceProvider, TRequestMessage> configureHandler)
        where TRequestMessage : class, IDaprRequestMessage
    {
        ArgumentNullException.ThrowIfNull(nameof(builder));

        ArgumentNullException.ThrowIfNull(configureHandler);

        builder.Services.TryAddEnumerable(new ServiceDescriptor(typeof(IDaprRequestMessage), configureHandler, ServiceLifetime.Singleton));

        builder.Services.Configure<CallerDaprClientOptions>(builder.Name, options =>
        {
            options.HttpRequestMessageActions.Add(b => b.RequestMessages.Add(configureHandler.Invoke(b.ServiceProvider)));
        });

        return builder;
    }
}
