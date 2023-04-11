// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public static class CallerBuilderExtensions
{
    public static MasaDaprClientBuilder UseDapr(this CallerBuilder callerBuilder,
        Action<MasaDaprClient> masDaprClientConfigure,
        Action<DaprClientBuilder>? configure = null)
    {
        MasaArgumentException.ThrowIfNull(masDaprClientConfigure);

        callerBuilder.Services.AddDaprClient(configure);

        return callerBuilder.UseDaprCore(() =>
        {
            callerBuilder.UseCustomCaller(serviceProvider =>
            {
                var masaDaprClient = new MasaDaprClient(serviceProvider);
                masDaprClientConfigure.Invoke(masaDaprClient);
                var appid = serviceProvider.GetRequiredService<ICallerProvider>().CompletionAppId(masaDaprClient.AppId);

                return new DaprCaller(
                    serviceProvider,
                    callerBuilder.Name,
                    appid,
                    masaDaprClient.RequestMessageFactory,
                    masaDaprClient.ResponseMessageFactory);
            });
        });
    }

    private static MasaDaprClientBuilder UseDaprCore(this CallerBuilder callerBuilder,
        Action action)
    {
        callerBuilder.Services.TryAddSingleton<ICallerProvider, DefaultCallerProvider>();
        callerBuilder.Services.AddOptions();
        action.Invoke();
        return new MasaDaprClientBuilder(callerBuilder.Services, callerBuilder.Name);
    }
}
