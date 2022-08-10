// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.DaprClient;

public static class CallerOptionsExtensions
{
    public static DefaultDaprClientBuilder UseDapr(this CallerOptions callerOptions,
        Func<MasaDaprClientBuilder> clientBuilder)
        => callerOptions.UseDapr(Microsoft.Extensions.Options.Options.DefaultName, clientBuilder);

    public static DefaultDaprClientBuilder UseDapr(this CallerOptions callerOptions,
        string name,
        Func<MasaDaprClientBuilder> clientBuilder)
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

        AddCallerExtensions.AddCaller(callerOptions, name,
            serviceProvider => new DaprCaller(serviceProvider, name, builder.AppId));
        return new DefaultDaprClientBuilder(callerOptions.Services, name);
    }

    public static DefaultDaprClientBuilder UseDapr(this CallerOptions callerOptions,
        Action<MasaDaprClientBuilder> clientBuilder)
        => callerOptions.UseDapr(Microsoft.Extensions.Options.Options.DefaultName, clientBuilder);

    public static DefaultDaprClientBuilder UseDapr(this CallerOptions callerOptions,
        string name,
        Action<MasaDaprClientBuilder> clientBuilder)
    {
        if (clientBuilder == null)
            throw new ArgumentNullException(nameof(clientBuilder));

        MasaDaprClientBuilder builder = new MasaDaprClientBuilder();
        clientBuilder.Invoke(builder);

        return callerOptions.UseDapr(name, () => builder);
    }
}
