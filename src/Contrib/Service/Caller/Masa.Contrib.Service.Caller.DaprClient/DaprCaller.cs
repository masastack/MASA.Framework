// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.DaprClient;

public class DaprCaller : AbstractCaller
{
    private Dapr.Client.DaprClient? _daprClient;
    private Dapr.Client.DaprClient DaprClient => _daprClient ??= ServiceProvider.GetRequiredService<Dapr.Client.DaprClient>();
    private readonly CallerDaprClientOptions _callerDaprClientOptions;
    protected readonly string AppId;

    public DaprCaller(IServiceProvider serviceProvider, string name, string appId)
        : base(serviceProvider)
    {
        var optionsFactory = serviceProvider.GetRequiredService<IOptionsFactory<CallerDaprClientOptions>>();
        _callerDaprClientOptions = optionsFactory.Create(name);
        AppId = appId;
        var logger = serviceProvider.GetService<ILogger<DaprCaller>>();
        logger?.LogDebug("The Name of the initialized Caller is {Name}, and the AppId is {AppId}", name, appId);
    }

    public override async Task<TResponse?> SendAsync<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken = default)
        where TResponse : default
    {
        var response = await DaprClient.InvokeMethodWithResponseAsync(request, cancellationToken);
        return await ResponseMessage.ProcessResponseAsync<TResponse>(response, cancellationToken);
    }

    public override async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string? methodName)
    {
        var httpRequestMessage =
            await RequestMessage.ProcessHttpRequestMessageAsync(DaprClient.CreateInvokeMethodRequest(method, AppId, methodName));

        if (RequestMessageFunc != null)
        {
            await RequestMessageFunc.Invoke(httpRequestMessage);
        }

        DealRequestMessage(Action);

        return httpRequestMessage;

        async void Action(IDaprRequestMessage requestMessage)
        {
            await requestMessage.ProcessHttpRequestMessageAsync(httpRequestMessage);
        }
    }

    public override async Task<HttpRequestMessage> CreateRequestAsync<TRequest>(HttpMethod method, string? methodName, TRequest data)
    {
        var httpRequestMessage =
            await RequestMessage.ProcessHttpRequestMessageAsync(DaprClient.CreateInvokeMethodRequest(method, AppId, methodName), data);

        if (RequestMessageFunc != null)
        {
            await RequestMessageFunc.Invoke(httpRequestMessage);
        }

        DealRequestMessage(Action);

        return httpRequestMessage;

        async void Action(IDaprRequestMessage requestMessage)
        {
            await requestMessage.ProcessHttpRequestMessageAsync(httpRequestMessage);
        }
    }

    private void DealRequestMessage(Action<IDaprRequestMessage> action)
    {
        foreach (var httpRequestMessageAction in _callerDaprClientOptions.HttpRequestMessageActions)
        {
            MasaHttpMessageHandlerBuilder masaHttpMessageHandlerBuilder = new MasaHttpMessageHandlerBuilder(ServiceProvider);
            httpRequestMessageAction.Invoke(masaHttpMessageHandlerBuilder);

            foreach (var requestMessage in masaHttpMessageHandlerBuilder.RequestMessages)
            {
                action.Invoke(requestMessage);
            }
        }
    }

    public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        return await DaprClient.InvokeMethodWithResponseAsync(request, cancellationToken);
    }

    public override Task SendGrpcAsync(string methodName, CancellationToken cancellationToken = default)
        => DaprClient.InvokeMethodGrpcAsync(AppId, methodName, cancellationToken);

    public override Task<TResponse> SendGrpcAsync<TResponse>(string methodName, CancellationToken cancellationToken = default)
        => DaprClient.InvokeMethodGrpcAsync<TResponse>(AppId, methodName, cancellationToken);

    public override Task SendGrpcAsync<TRequest>(string methodName, TRequest request, CancellationToken cancellationToken = default)
        => DaprClient.InvokeMethodGrpcAsync(AppId, methodName, request, cancellationToken);

    public override Task<TResponse> SendGrpcAsync<TRequest, TResponse>(
        string methodName,
        TRequest request,
        CancellationToken cancellationToken = default)
        => DaprClient.InvokeMethodGrpcAsync<TResponse>(AppId, methodName, cancellationToken);
}
