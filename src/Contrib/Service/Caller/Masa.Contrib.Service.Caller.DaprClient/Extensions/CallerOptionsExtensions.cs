// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public static class CallerOptionsExtensions
{
    private static readonly string DefaultCallerName = Options.DefaultName;

    public static MasaDaprClientBuilder UseDapr(this CallerOptions callerOptions,
        Func<MasaDaprClient> clientBuilder)
        => callerOptions.UseDapr(DefaultCallerName, clientBuilder);

    public static MasaDaprClientBuilder UseDapr(this CallerOptions callerOptions,
        string name,
        Func<MasaDaprClient> clientBuilder)
    {
        return callerOptions.UseDaprCore(name, () =>
        {
            ArgumentNullException.ThrowIfNull(clientBuilder, nameof(ArgumentNullException));
            var builder = clientBuilder.Invoke();

            callerOptions.Services.AddDaprClient(daprClientBuilder =>
            {
                var jsonSerializerOptions = builder.JsonSerializerOptions ?? MasaApp.GetJsonSerializerOptions();
                if (jsonSerializerOptions != null)
                    daprClientBuilder.UseJsonSerializationOptions(jsonSerializerOptions);

                builder.Configure?.Invoke(daprClientBuilder);
            });

            AddCallerExtensions.AddCaller(callerOptions, name,
                serviceProvider =>
                {
                    var appid = serviceProvider.GetRequiredService<ICallerProvider>().CompletionAppId(builder.AppId);
                    var daprCaller = new DaprCaller(serviceProvider,
                        name,
                        appid,
                        builder.RequestMessageFactory,
                        builder.ResponseMessageFactory);
                    return daprCaller;
                });
        });
    }

    public static MasaDaprClientBuilder UseDapr(this CallerOptions callerOptions,
        string name,
        Action<MasaDaprClient> clientBuilder)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder, nameof(clientBuilder));

        MasaDaprClient builder = new MasaDaprClient();
        clientBuilder.Invoke(builder);

        return callerOptions.UseDapr(name, () => builder);
    }

    public static MasaDaprClientBuilder UseDapr(this CallerOptions callerOptions,
        Action<MasaDaprClient> clientBuilder)
        => callerOptions.UseDapr(DefaultCallerName, clientBuilder);

    private static MasaDaprClientBuilder UseDaprCore(this CallerOptions callerOptions,
        string name,
        Action action)
    {
        callerOptions.Services.TryAddSingleton<ICallerProvider, DefaultCallerProvider>();
        callerOptions.Services.AddOptions();
        action.Invoke();
        return new MasaDaprClientBuilder(callerOptions.Services, name);
    }
}
