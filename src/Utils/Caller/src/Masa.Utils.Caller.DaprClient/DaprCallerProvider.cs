// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.DaprClient;

public class DaprCallerProvider : AbstractCallerProvider
{
    private readonly string AppId;
    private Dapr.Client.DaprClient? _daprClient;
    private Dapr.Client.DaprClient DaprClient => _daprClient ??= ServiceProvider.GetRequiredService<Dapr.Client.DaprClient>();

    public DaprCallerProvider(IServiceProvider serviceProvider, string appId)
        : base(serviceProvider)
        => AppId = appId;

    public override async Task<TResponse?> SendAsync<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken = default)
        where TResponse : default
    {
        var response = await DaprClient.InvokeMethodWithResponseAsync(request, cancellationToken);
        return await ResponseMessage.ProcessResponseAsync<TResponse>(response, cancellationToken);
    }

    public override Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string? methodName)
        => RequestMessage.ProcessHttpRequestMessageAsync(DaprClient.CreateInvokeMethodRequest(method, AppId, methodName));

    public override Task<HttpRequestMessage> CreateRequestAsync<TRequest>(HttpMethod method, string? methodName, TRequest data)
        => RequestMessage.ProcessHttpRequestMessageAsync(DaprClient.CreateInvokeMethodRequest(method, AppId, methodName), data);

    public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        => DaprClient.InvokeMethodWithResponseAsync(request, cancellationToken);

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
