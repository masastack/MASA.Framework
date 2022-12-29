// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public static class CallerOptionsExtensions
{
    private static readonly string DefaultCallerName = Options.DefaultName;

    public static DefaultDaprClientBuilder UseDapr(this CallerOptions callerOptions,
        Func<MasaDaprClientBuilder> clientBuilder)
        => callerOptions.UseDapr(DefaultCallerName, clientBuilder);

    public static DefaultDaprClientBuilder UseDapr(this CallerOptions callerOptions,
        string name,
        Func<MasaDaprClientBuilder> clientBuilder)
    {
        return callerOptions.UseDaprCore(name, () =>
        {
            ArgumentNullException.ThrowIfNull(clientBuilder, nameof(ArgumentNullException));
            var builder = clientBuilder.Invoke();

            callerOptions.Services.AddDaprClient(daprClientBuilder =>
            {
                if (callerOptions.JsonSerializerOptions != null)
                    daprClientBuilder.UseJsonSerializationOptions(callerOptions.JsonSerializerOptions);

                builder.Configure?.Invoke(daprClientBuilder);
            });

            AddCallerExtensions.AddCaller(callerOptions, name,
                serviceProvider =>
                {
                    var appid = serviceProvider.GetRequiredService<ICallerProvider>().CompletionAppId(builder.AppId);
                    var daprCaller = new DaprCaller(serviceProvider,
                        name,
                        appid);
                    return daprCaller;
                });
        });
    }

    public static DefaultDaprClientBuilder UseDapr(this CallerOptions callerOptions,
        Action<MasaDaprClientBuilder> clientBuilder)
        => callerOptions.UseDapr(DefaultCallerName, clientBuilder);

    public static DefaultDaprClientBuilder UseDapr(this CallerOptions callerOptions,
        string name,
        Action<MasaDaprClientBuilder> clientBuilder)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder, nameof(clientBuilder));

        MasaDaprClientBuilder builder = new MasaDaprClientBuilder();
        clientBuilder.Invoke(builder);

        return callerOptions.UseDapr(name, () => builder);
    }

    public static DefaultDaprClientBuilder UseDaprTest(this CallerOptions callerOptions,
        string name,
        string appId,
        DaprClient daprClient)
    {
        return callerOptions.UseDaprCore(name, () =>
        {
            AddCallerExtensions.AddCaller(callerOptions, name,
                serviceProvider =>
                {
                    var appid = serviceProvider.GetRequiredService<ICallerProvider>().CompletionAppId(appId);
                    var daprCaller = new DaprCaller(serviceProvider,
                        daprClient,
                        name,
                        appid);
                    return daprCaller;
                });
        });
    }

    private static DefaultDaprClientBuilder UseDaprCore(this CallerOptions callerOptions,
        string name,
        Action action)
    {
        callerOptions.Services.TryAddSingleton<ICallerProvider, DefaultCallerProvider>();
        callerOptions.Services.AddOptions();
        action.Invoke();
        return new DefaultDaprClientBuilder(callerOptions.Services, name);
    }
}
