// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.Core;

public abstract class AbstractCallerProvider : ICallerProvider
{
    private readonly ITypeConvertProvider _typeConvertProvider;
    public readonly IServiceProvider ServiceProvider;

    private IRequestMessage? _requestMessage;
    private IResponseMessage? _responseMessage;
    protected IRequestMessage RequestMessage => _requestMessage ??= ServiceProvider.GetRequiredService<IRequestMessage>();
    protected IResponseMessage ResponseMessage => _responseMessage ??= ServiceProvider.GetRequiredService<IResponseMessage>();

    public AbstractCallerProvider(IServiceProvider serviceProvider)
    {
        _typeConvertProvider = serviceProvider.GetRequiredService<ITypeConvertProvider>();
        ServiceProvider = serviceProvider;
    }

    public virtual async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default)
    {
        var response = await SendAsync(request, cancellationToken);
        if (autoThrowUserFriendlyException)
            await ResponseMessage.ProcessResponseAsync(response, cancellationToken);

        return response;
    }

    public abstract Task<TResponse?> SendAsync<TResponse>(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default);

    public abstract Task<HttpRequestMessage> CreateRequestAsync(
        HttpMethod method,
        string? methodName);

    public abstract Task<HttpRequestMessage> CreateRequestAsync<TRequest>(
        HttpMethod method,
        string? methodName,
        TRequest data);

    public abstract Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default);

    public virtual async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string? methodName,
        HttpContent? content,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage request = await CreateRequestAsync(method, methodName);
        request.Content = content;
        return await SendAsync(request, autoThrowUserFriendlyException, cancellationToken);
    }

    public virtual async Task<HttpResponseMessage> SendAsync<TRequest>(
        HttpMethod method,
        string? methodName,
        TRequest data,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage request = await CreateRequestAsync(method, methodName, data);
        return await SendAsync(request, autoThrowUserFriendlyException, cancellationToken);
    }

    public virtual async Task<TResponse?> SendAsync<TRequest, TResponse>(
        HttpMethod method,
        string? methodName,
        TRequest data,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage request = await CreateRequestAsync(method, methodName, data);
        return await SendAsync<TResponse>(request, cancellationToken);
    }

    public abstract Task SendGrpcAsync(
        string methodName,
        CancellationToken cancellationToken = default);

    public abstract Task<TResponse> SendGrpcAsync<TResponse>(
        string methodName,
        CancellationToken cancellationToken = default)
        where TResponse : IMessage, new();

    public abstract Task SendGrpcAsync<TRequest>(
        string methodName,
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : IMessage;

    public abstract Task<TResponse> SendGrpcAsync<TRequest, TResponse>(
        string methodName,
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : IMessage
        where TResponse : IMessage, new();

    public virtual async Task<string> GetStringAsync(
        string? methodName,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage request = await CreateRequestAsync(HttpMethod.Get, methodName);
        HttpResponseMessage content = await SendAsync(request, autoThrowUserFriendlyException, cancellationToken);
        return await content.Content.ReadAsStringAsync(cancellationToken);
    }

    public virtual Task<string> GetStringAsync<TRequest>(
        string? methodName,
        TRequest data,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default) where TRequest : class
        => GetStringAsync(
            GetUrl(methodName, _typeConvertProvider.ConvertToKeyValuePairs(data)),
            autoThrowUserFriendlyException,
            cancellationToken);

    public virtual Task<string> GetStringAsync(
        string? methodName,
        Dictionary<string, string> data,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default)
        => GetStringAsync(GetUrl(methodName, data), autoThrowUserFriendlyException, cancellationToken);

    public virtual async Task<byte[]> GetByteArrayAsync(
        string? methodName,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage request = await CreateRequestAsync(HttpMethod.Get, methodName);
        HttpResponseMessage content = await SendAsync(request, autoThrowUserFriendlyException, cancellationToken);
        return await content.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    public virtual Task<byte[]> GetByteArrayAsync<TRequest>(
        string? methodName,
        TRequest data,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default) where TRequest : class
        => GetByteArrayAsync(
            GetUrl(methodName, _typeConvertProvider.ConvertToKeyValuePairs(data)),
            autoThrowUserFriendlyException,
            cancellationToken);

    public virtual Task<byte[]> GetByteArrayAsync(
        string? methodName,
        Dictionary<string, string> data,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default)
        => GetByteArrayAsync(GetUrl(methodName, data), autoThrowUserFriendlyException, cancellationToken);

    public virtual async Task<Stream> GetStreamAsync(
        string? methodName,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage request = await CreateRequestAsync(HttpMethod.Get, methodName);
        HttpResponseMessage content = await SendAsync(request, autoThrowUserFriendlyException, cancellationToken);
        return await content.Content.ReadAsStreamAsync(cancellationToken);
    }

    public virtual Task<Stream> GetStreamAsync<TRequest>(
        string? methodName,
        TRequest data,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default) where TRequest : class
        => GetStreamAsync(
            GetUrl(methodName, _typeConvertProvider.ConvertToKeyValuePairs(data)),
            autoThrowUserFriendlyException,
            cancellationToken);

    public virtual Task<Stream> GetStreamAsync(
        string? methodName,
        Dictionary<string, string> data,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default)
        => GetStreamAsync(GetUrl(methodName, data), autoThrowUserFriendlyException, cancellationToken);

    public virtual Task<HttpResponseMessage> GetAsync(
        string? methodName,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Get, methodName, null, autoThrowUserFriendlyException, cancellationToken);

    public virtual Task<HttpResponseMessage> GetAsync(
        string? methodName,
        Dictionary<string, string> data,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default)
        => GetAsync(GetUrl(methodName, data), autoThrowUserFriendlyException, cancellationToken);

    public virtual async Task<TResponse?> GetAsync<TResponse>(
        string? methodName,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage request = await CreateRequestAsync(HttpMethod.Get, methodName);
        return await SendAsync<TResponse>(request, cancellationToken);
    }

    public virtual async Task<TResponse?> GetAsync<TRequest, TResponse>(
        string? methodName,
        TRequest data,
        CancellationToken cancellationToken = default) where TRequest : class
    {
        HttpRequestMessage request =
            await CreateRequestAsync(HttpMethod.Get, GetUrl(methodName, _typeConvertProvider.ConvertToKeyValuePairs(data)));
        return await SendAsync<TResponse>(request, cancellationToken);
    }

    public virtual async Task<TResponse?> GetAsync<TResponse>(
        string? methodName,
        object data,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage request =
            await CreateRequestAsync(HttpMethod.Get, GetUrl(methodName, _typeConvertProvider.ConvertToKeyValuePairs(data)));
        return await SendAsync<TResponse>(request, cancellationToken);
    }

    public virtual async Task<TResponse?> GetAsync<TResponse>(
        string? methodName,
        Dictionary<string, string> data,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage request = await CreateRequestAsync(HttpMethod.Get, GetUrl(methodName, data));
        return await SendAsync<TResponse>(request, cancellationToken);
    }

    protected virtual string GetUrl(string? url, IEnumerable<KeyValuePair<string, string>> properties)
    {
        url ??= string.Empty;
        foreach (var property in properties)
        {
            string value = property.Value;

            url = !url.Contains("?") ?
                $"{url}?{property.Key}={value}" :
                $"{url}&{property.Key}={value}";
        }

        return url;
    }

    public virtual Task<HttpResponseMessage> PostAsync(
        string? methodName,
        HttpContent? content,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Post, methodName, content, autoThrowUserFriendlyException, cancellationToken);

    public virtual async Task<HttpResponseMessage> PostAsync<TRequest>(
        string? methodName,
        TRequest data,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default)
    {
        var request = await CreateRequestAsync(HttpMethod.Post, methodName, data);
        return await SendAsync(request, autoThrowUserFriendlyException, cancellationToken);
    }

    public virtual async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string? methodName,
        TRequest data,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage request = await CreateRequestAsync(HttpMethod.Post, methodName, data);
        return await SendAsync<TResponse>(request, cancellationToken);
    }

    public virtual async Task<TResponse?> PostAsync<TResponse>(
        string? methodName,
        object data,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage request = await CreateRequestAsync(HttpMethod.Post, methodName, data);
        return await SendAsync<TResponse>(request, cancellationToken);
    }

    public virtual Task<HttpResponseMessage> PatchAsync(
        string? methodName,
        HttpContent? content,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Patch, methodName, content, autoThrowUserFriendlyException, cancellationToken);

    public virtual async Task<HttpResponseMessage> PatchAsync<TRequest>(
        string? methodName,
        TRequest data,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default)
    {
        var request = await CreateRequestAsync(HttpMethod.Patch, methodName, data);
        return await SendAsync(request, autoThrowUserFriendlyException, cancellationToken);
    }

    public virtual async Task<TResponse?> PatchAsync<TRequest, TResponse>(
        string? methodName,
        TRequest data,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage request = await CreateRequestAsync(HttpMethod.Post, methodName, data);
        return await SendAsync<TResponse>(request, cancellationToken);
    }

    public virtual async Task<TResponse?> PatchAsync<TResponse>(
        string? methodName,
        object data,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage request = await CreateRequestAsync(HttpMethod.Post, methodName, data);
        return await SendAsync<TResponse>(request, cancellationToken);
    }

    public virtual Task<HttpResponseMessage> PutAsync(
        string? methodName,
        HttpContent? content,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Put, methodName, content, autoThrowUserFriendlyException, cancellationToken);

    public virtual async Task<HttpResponseMessage> PutAsync<TRequest>(
        string? methodName,
        TRequest data,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default)
    {
        var request = await CreateRequestAsync(HttpMethod.Put, methodName, data);
        return await SendAsync(request, autoThrowUserFriendlyException, cancellationToken);
    }

    public virtual async Task<TResponse?> PutAsync<TRequest, TResponse>(
        string? methodName,
        TRequest data,
        CancellationToken cancellationToken = default)
    {
        var request = await CreateRequestAsync(HttpMethod.Put, methodName, data);
        return await SendAsync<TResponse>(request, cancellationToken);
    }

    public virtual async Task<TResponse?> PutAsync<TResponse>(
        string? methodName,
        object data,
        CancellationToken cancellationToken = default)
    {
        var request = await CreateRequestAsync(HttpMethod.Put, methodName, data);
        return await SendAsync<TResponse>(request, cancellationToken);
    }

    public virtual Task<HttpResponseMessage> DeleteAsync(
        string? methodName,
        HttpContent? content,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Delete, methodName, content, autoThrowUserFriendlyException, cancellationToken);

    public virtual async Task<HttpResponseMessage> DeleteAsync<TRequest>(
        string? methodName,
        TRequest data,
        bool autoThrowUserFriendlyException = true,
        CancellationToken cancellationToken = default)
    {
        var request = await CreateRequestAsync(HttpMethod.Delete, methodName, data);
        return await SendAsync(request, autoThrowUserFriendlyException, cancellationToken);
    }

    public virtual async Task<TResponse?> DeleteAsync<TRequest, TResponse>(
        string? methodName,
        TRequest data,
        CancellationToken cancellationToken = default)
    {
        var request = await CreateRequestAsync(HttpMethod.Delete, methodName, data);
        return await SendAsync<TResponse>(request, cancellationToken);
    }

    public virtual async Task<TResponse?> DeleteAsync<TResponse>(
        string? methodName,
        object data,
        CancellationToken cancellationToken = default)
    {
        var request = await CreateRequestAsync(HttpMethod.Delete, methodName, data);
        return await SendAsync<TResponse>(request, cancellationToken);
    }
}
