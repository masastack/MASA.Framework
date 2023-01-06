// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public static class DefaultDaprClientBuilderExtensions
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

    public static DefaultDaprClientBuilder AddHttpRequestMessage<TRequestMessage>(
        this DefaultDaprClientBuilder builder,
        Func<IServiceProvider, TRequestMessage> configureHandler)
        where TRequestMessage : class, IDaprRequestMessage
    {
        ArgumentNullException.ThrowIfNull(nameof(builder));

        ArgumentNullException.ThrowIfNull(configureHandler);

        builder.Services.Configure<CallerDaprClientOptions>(builder.Name, options =>
        {
            options.HttpRequestMessageActions.Add(b => b.RequestMessages.Add(configureHandler.Invoke(b.ServiceProvider)));
        });

        return builder;
    }
}
