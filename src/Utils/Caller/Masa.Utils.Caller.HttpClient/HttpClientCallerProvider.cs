// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.HttpClient;

public class HttpClientCallerProvider : AbstractCallerProvider
{
    private readonly System.Net.Http.HttpClient _httpClient;
    private readonly string _prefix;
    private readonly bool _prefixIsNullOrEmpty;

    public HttpClientCallerProvider(IServiceProvider serviceProvider, string name, string prefix)
        : base(serviceProvider)
    {
        _httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(name);
        _prefix = prefix;
        _prefixIsNullOrEmpty = string.IsNullOrEmpty(_prefix);
    }

    public override async Task<TResponse?> SendAsync<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken = default)
        where TResponse : default
    {
        var response = await _httpClient.SendAsync(request, cancellationToken);
        return await ResponseMessage.ProcessResponseAsync<TResponse>(response, cancellationToken);
    }

    public override Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string? methodName)
        => RequestMessage.ProcessHttpRequestMessageAsync(new HttpRequestMessage(method, GetRequestUri(methodName)));

    public override Task<HttpRequestMessage> CreateRequestAsync<TRequest>(HttpMethod method, string? methodName, TRequest data)
        => RequestMessage.ProcessHttpRequestMessageAsync(new HttpRequestMessage(method, GetRequestUri(methodName)), data);

    public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        return _httpClient.SendAsync(request, cancellationToken);
    }

    public override Task SendGrpcAsync(string methodName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<TResponse> SendGrpcAsync<TResponse>(string methodName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task SendGrpcAsync<TRequest>(string methodName, TRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<TResponse> SendGrpcAsync<TRequest, TResponse>(string methodName, TRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    protected virtual string GetRequestUri(string? methodName)
    {
        if (string.IsNullOrEmpty(methodName))
            return string.Empty;

        if (Uri.IsWellFormedUriString(methodName, UriKind.Absolute) || _prefixIsNullOrEmpty)
            return methodName;

        if (_prefix.EndsWith("/"))
            return $"{_prefix}{(methodName.StartsWith("/") ? methodName.Substring(1) : methodName)}";

        return $"{_prefix}{(methodName.StartsWith("/") ? methodName : "/" + methodName)}";
    }
}
