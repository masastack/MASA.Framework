// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public static class CallerOptionsExtensions
{
    public static MasaDaprClientBuilder UseDapr(this CallerOptionsBuilder callerOptionsBuilder,
        Action<MasaDaprClient> masDaprClientConfigure,
        Action<DaprClientBuilder>? configure = null)
    {
        MasaArgumentException.ThrowIfNull(masDaprClientConfigure);

        callerOptionsBuilder.Services.AddDaprClient(configure);

        return callerOptionsBuilder.UseDaprCore(() =>
        {
            callerOptionsBuilder.Services.AddCaller(callerOptionsBuilder.Name, serviceProvider =>
            {
                var masaDaprClient = new MasaDaprClient();
                masDaprClientConfigure.Invoke(masaDaprClient);
                var appid = serviceProvider.GetRequiredService<ICallerProvider>().CompletionAppId(masaDaprClient.AppId);

                return new DaprCaller(
                    serviceProvider,
                    callerOptionsBuilder.Name,
                    appid,
                    masaDaprClient.RequestMessageFactory,
                    masaDaprClient.ResponseMessageFactory);
            });
        });
    }

    private static MasaDaprClientBuilder UseDaprCore(this CallerOptionsBuilder callerOptionsBuilder,
        Action action)
    {
        callerOptionsBuilder.Services.TryAddSingleton<ICallerProvider, DefaultCallerProvider>();
        callerOptionsBuilder.Services.AddOptions();
        action.Invoke();
        return new MasaDaprClientBuilder(callerOptionsBuilder.Services, callerOptionsBuilder.Name);
    }
}
