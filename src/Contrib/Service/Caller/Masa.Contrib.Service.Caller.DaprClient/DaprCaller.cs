// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.DaprClient;

public class DaprCaller : AbstractCaller
{
    private Dapr.Client.DaprClient? _daprClient;
    private Dapr.Client.DaprClient DaprClient => _daprClient ??= ServiceProvider.GetRequiredService<Dapr.Client.DaprClient>();
    protected readonly string AppId;

    public DaprCaller(
        IServiceProvider serviceProvider,
        string name,
        string appId,
        Func<IServiceProvider, IRequestMessage>? requestMessageFactory,
        Func<IServiceProvider, IResponseMessage>? responseMessageFactory)
        : this(serviceProvider, null, name, appId, requestMessageFactory, responseMessageFactory)
    {
    }

    public DaprCaller(
        IServiceProvider serviceProvider,
        Dapr.Client.DaprClient? daprClient,
        string name,
        string appId,
        Func<IServiceProvider, IRequestMessage>? requestMessageFactory,
        Func<IServiceProvider, IResponseMessage>? responseMessageFactory)
        : base(serviceProvider, name, requestMessageFactory, responseMessageFactory)
    {
        AppId = appId;
        _daprClient = daprClient;
        var logger = serviceProvider.GetService<ILogger<DaprCaller>>();
        logger?.LogDebug("The Name of the initialized Caller is {Name}, and the AppId is {AppId}", name, appId);
    }

    public override HttpRequestMessage CreateRequest(HttpMethod method, string? methodName)
    {
        var requestMessage = DaprClient.CreateInvokeMethodRequest(method, AppId, methodName);
        RequestMessage.ProcessHttpRequestMessage(requestMessage);
        return requestMessage;
    }

    public override HttpRequestMessage CreateRequest<TRequest>(HttpMethod method, string? methodName, TRequest data)
    {
        var requestMessage = DaprClient.CreateInvokeMethodRequest(method, AppId, methodName);
        RequestMessage.ProcessHttpRequestMessage(requestMessage, data);
        return requestMessage;
    }

    public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        => await DaprClient.InvokeMethodWithResponseAsync(request, cancellationToken);

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
