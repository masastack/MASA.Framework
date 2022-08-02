// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.DaprClient;

public static class CallerOptionsExtensions
{
    public static CallerOptions UseDapr(this CallerOptions callerOptions, Func<MasaDaprClientBuilder> clientBuilder)
    {
        if (clientBuilder == null)
            throw new ArgumentNullException(nameof(clientBuilder));

        MasaDaprClientBuilder builder = clientBuilder.Invoke();
        if (clientBuilder == null)
            throw new ArgumentNullException(nameof(clientBuilder));

        callerOptions.Services.AddDaprClient(daprClientBuilder =>
        {
            if (callerOptions.JsonSerializerOptions != null)
                daprClientBuilder.UseJsonSerializationOptions(callerOptions.JsonSerializerOptions);

            builder.Configure?.Invoke(daprClientBuilder);
        });

        AddCallerExtensions.AddCaller(callerOptions, builder.Name, builder.IsDefault,
            serviceProvider => new DaprCallerProvider(serviceProvider, builder.AppId));
        return callerOptions;
    }

    public static CallerOptions UseDapr(this CallerOptions callerOptions, Action<MasaDaprClientBuilder> clientBuilder)
    {
        if (clientBuilder == null)
            throw new ArgumentNullException(nameof(clientBuilder));

        MasaDaprClientBuilder builder = new MasaDaprClientBuilder();
        clientBuilder.Invoke(builder);

        return callerOptions.UseDapr(() => builder);
    }
}
