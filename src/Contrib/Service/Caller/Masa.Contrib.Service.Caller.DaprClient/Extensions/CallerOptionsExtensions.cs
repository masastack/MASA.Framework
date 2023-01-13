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
        Func<MasaDaprClient> configureClient,
        Action<DaprClientBuilder>? configure = null,
        bool alwaysGetNewestDaprClient = false)
    {
        return callerOptions.UseDapr(name, client =>
        {
            var masaDaprClient = configureClient.Invoke();
            client.AppId = masaDaprClient.AppId;
            client.RequestMessageFactory = masaDaprClient.RequestMessageFactory;
            client.ResponseMessageFactory = masaDaprClient.ResponseMessageFactory;
        }, configure, alwaysGetNewestDaprClient);
    }

    public static MasaDaprClientBuilder UseDapr(this CallerOptions callerOptions,
        Action<MasaDaprClient> masDaprClientConfigure,
        Action<DaprClientBuilder>? configure = null,
        bool alwaysGetNewestDaprClient = false)
        => callerOptions.UseDapr(DefaultCallerName, masDaprClientConfigure, configure, alwaysGetNewestDaprClient);

    public static MasaDaprClientBuilder UseDapr(this CallerOptions callerOptions,
        string name,
        Action<MasaDaprClient> masDaprClientConfigure,
        Action<DaprClientBuilder>? configure = null,
        bool alwaysGetNewestDaprClient = false)
    {
        callerOptions.Services.AddDaprClient(configure);

        return callerOptions.UseDaprCore(name, () =>
        {
            callerOptions.Services.AddDaprClient(configure);
            if (alwaysGetNewestDaprClient)
            {
                AddCallerExtensions.AddCaller(callerOptions, name, serviceProvider =>
                {
                    var masaDaprClient = new MasaDaprClient();
                    masDaprClientConfigure.Invoke(masaDaprClient);
                    var appid = serviceProvider.GetRequiredService<ICallerProvider>().CompletionAppId(masaDaprClient.AppId);

                    return new DaprCaller(
                        serviceProvider,
                        name,
                        appid,
                        masaDaprClient.RequestMessageFactory,
                        masaDaprClient.ResponseMessageFactory);
                });
            }
            else
            {
                var masaDaprClient = new MasaDaprClient();
                masDaprClientConfigure.Invoke(masaDaprClient);

                AddCallerExtensions.AddCaller(callerOptions, name, serviceProvider =>
                {
                    var appid = serviceProvider.GetRequiredService<ICallerProvider>().CompletionAppId(masaDaprClient.AppId);

                    return new DaprCaller(
                        serviceProvider,
                        name,
                        appid,
                        masaDaprClient.RequestMessageFactory,
                        masaDaprClient.ResponseMessageFactory);
                });
            }
        });
    }

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
