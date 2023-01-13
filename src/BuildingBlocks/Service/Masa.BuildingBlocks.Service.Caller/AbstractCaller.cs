// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Caller;

public abstract class AbstractCaller : ICaller
{
    private readonly ITypeConvertor _typeConvertor;
    protected readonly IServiceProvider ServiceProvider;

    private readonly Func<IServiceProvider, IRequestMessage>? _requestMessageFactory;
    private readonly Func<IServiceProvider, IResponseMessage>? _responseMessageFactory;

    private IRequestMessage? _requestMessage;
    private IResponseMessage? _responseMessage;

    protected IRequestMessage RequestMessage => _requestMessage ??=
        _requestMessageFactory?.Invoke(ServiceProvider) ?? ServiceProvider.GetRequiredService<IRequestMessage>();

    protected IResponseMessage ResponseMessage => _responseMessage ??=
        _responseMessageFactory?.Invoke(ServiceProvider) ?? ServiceProvider.GetRequiredService<IResponseMessage>();

    protected readonly List<Func<IServiceProvider, ICallerMiddleware>> Middlewares;

    protected AbstractCaller(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    protected AbstractCaller(IServiceProvider serviceProvider,
        string name,
        Func<IServiceProvider, IRequestMessage>? currentRequestMessageFactory,
        Func<IServiceProvider, IResponseMessage>? currentResponseMessageFactory) : this(serviceProvider)
    {
        _requestMessageFactory = currentRequestMessageFactory;
        _responseMessageFactory = currentResponseMessageFactory;

        _typeConvertor = serviceProvider.GetRequiredService<ITypeConvertor>();

        var options = serviceProvider.GetRequiredService<IOptions<CallerMiddlewareFactoryOptions>>().Value
            .Options
            .FirstOrDefault(relationOptions => relationOptions.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        Middlewares = options?.Middlewares.ToArray().Reverse().ToList() ?? new List<Func<IServiceProvider, ICallerMiddleware>>();
    }

    public virtual Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
        => SendAsync(() => request, autoThrowException, cancellationToken);

    public virtual Task<TResponse?> SendAsync<TResponse>(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default)
        => SendAsync<TResponse>(() => request, cancellationToken);

    public abstract Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default);

    public virtual Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string? methodName,
        HttpContent? content,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
        => SendAsync(() =>
        {
            var request = CreateRequest(method, methodName);
            request.Content = content;
            return request;
        }, autoThrowException, cancellationToken);

    public virtual Task<HttpResponseMessage> SendAsync<TRequest>(
        HttpMethod method,
        string? methodName,
        TRequest data,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
        => SendAsync(() => CreateRequest(method, methodName, data), autoThrowException, cancellationToken);

    public virtual Task<TResponse?> SendAsync<TRequest, TResponse>(
        HttpMethod method,
        string? methodName,
        TRequest data,
        CancellationToken cancellationToken = default)
        => SendAsync<TResponse?>(() => CreateRequest(method, methodName, data), cancellationToken);

    protected virtual async Task<HttpResponseMessage> SendAsync(
        Func<HttpRequestMessage> requestMessageFunc,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
    {
        var masaHttpContext = new MasaHttpContext(ServiceProvider, ResponseMessage, requestMessageFunc);
        CallerHandlerDelegate callerHandlerDelegate = async () =>
        {
            masaHttpContext.ResponseMessage = await SendAsync(masaHttpContext.RequestMessage, cancellationToken);

            if (autoThrowException) await masaHttpContext.ProcessResponseAsync(cancellationToken);
        };

        await Middlewares.Aggregate(callerHandlerDelegate,
            (next, func) => () => func.Invoke(ServiceProvider).HandleAsync(masaHttpContext, next, cancellationToken))();
        return masaHttpContext.ResponseMessage;
    }

    protected virtual async Task<TResponse?> SendAsync<TResponse>(
        Func<HttpRequestMessage> requestMessageFunc,
        CancellationToken cancellationToken = default)
    {
        TResponse? response = default;
        var masaHttpContext = new MasaHttpContext(ServiceProvider, ResponseMessage, requestMessageFunc);
        CallerHandlerDelegate callerHandlerDelegate = async () =>
        {
            masaHttpContext.ResponseMessage = await SendAsync(masaHttpContext.RequestMessage, cancellationToken);
            response = await masaHttpContext.ProcessResponseAsync<TResponse>(cancellationToken);
        };
        await Middlewares.Aggregate(callerHandlerDelegate,
            (next, func) => () => func.Invoke(ServiceProvider).HandleAsync(masaHttpContext, next, cancellationToken))();
        return response;
    }

    public abstract HttpRequestMessage CreateRequest(
        HttpMethod method,
        string? methodName);

    public abstract HttpRequestMessage CreateRequest<TRequest>(
        HttpMethod method,
        string? methodName,
        TRequest data);

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
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
    {
        string content = default!;
        var masaHttpContext = new MasaHttpContext(ServiceProvider, ResponseMessage, () => CreateRequest(HttpMethod.Get, methodName));
        CallerHandlerDelegate callerHandlerDelegate = async () =>
        {
            masaHttpContext.ResponseMessage = await SendAsync(masaHttpContext.RequestMessage, cancellationToken);

            if (autoThrowException) await masaHttpContext.ProcessResponseAsync(cancellationToken);

            content = await masaHttpContext.ResponseMessage.Content.ReadAsStringAsync(cancellationToken);
        };

        await Middlewares.Aggregate(callerHandlerDelegate,
            (next, func) => () => func.Invoke(ServiceProvider).HandleAsync(masaHttpContext, next, cancellationToken))();
        return content;
    }

    public virtual Task<string> GetStringAsync<TRequest>(
        string? methodName,
        TRequest data,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default) where TRequest : class
        => GetStringAsync(
            GetUrl(methodName, _typeConvertor.ConvertToKeyValuePairs(data)),
            autoThrowException,
            cancellationToken);

    public virtual Task<string> GetStringAsync(
        string? methodName,
        Dictionary<string, string> data,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
        => GetStringAsync(GetUrl(methodName, data), autoThrowException, cancellationToken);

    public virtual async Task<byte[]> GetByteArrayAsync(
        string? methodName,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
    {
        byte[] content = default!;
        var masaHttpContext = new MasaHttpContext(ServiceProvider, ResponseMessage, () => CreateRequest(HttpMethod.Get, methodName));
        CallerHandlerDelegate callerHandlerDelegate = async () =>
        {
            masaHttpContext.ResponseMessage = await SendAsync(masaHttpContext.RequestMessage, cancellationToken);

            if (autoThrowException) await masaHttpContext.ProcessResponseAsync(cancellationToken);

            content = await masaHttpContext.ResponseMessage.Content.ReadAsByteArrayAsync(cancellationToken);
        };
        await Middlewares.Aggregate(callerHandlerDelegate,
            (next, func) => () => func.Invoke(ServiceProvider).HandleAsync(masaHttpContext, next, cancellationToken))();
        return content;
    }

    public virtual Task<byte[]> GetByteArrayAsync<TRequest>(
        string? methodName,
        TRequest data,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default) where TRequest : class
        => GetByteArrayAsync(
            GetUrl(methodName, _typeConvertor.ConvertToKeyValuePairs(data)),
            autoThrowException,
            cancellationToken);

    public virtual Task<byte[]> GetByteArrayAsync(
        string? methodName,
        Dictionary<string, string> data,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
        => GetByteArrayAsync(GetUrl(methodName, data), autoThrowException, cancellationToken);

    public virtual async Task<Stream> GetStreamAsync(
        string? methodName,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
    {
        Stream content = default!;
        var masaHttpContext = new MasaHttpContext(ServiceProvider, ResponseMessage, () => CreateRequest(HttpMethod.Get, methodName));
        CallerHandlerDelegate callerHandlerDelegate = async () =>
        {
            masaHttpContext.ResponseMessage = await SendAsync(masaHttpContext.RequestMessage, cancellationToken);

            if (autoThrowException) await masaHttpContext.ProcessResponseAsync(cancellationToken);

            content = await masaHttpContext.ResponseMessage.Content.ReadAsStreamAsync(cancellationToken);
        };
        await Middlewares.Aggregate(callerHandlerDelegate,
            (next, func) => () => func.Invoke(ServiceProvider).HandleAsync(masaHttpContext, next, cancellationToken))();
        return content;
    }

    public virtual Task<Stream> GetStreamAsync<TRequest>(
        string? methodName,
        TRequest data,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default) where TRequest : class
        => GetStreamAsync(
            GetUrl(methodName, _typeConvertor.ConvertToKeyValuePairs(data)),
            autoThrowException,
            cancellationToken);

    public virtual Task<Stream> GetStreamAsync(
        string? methodName,
        Dictionary<string, string> data,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
        => GetStreamAsync(GetUrl(methodName, data), autoThrowException, cancellationToken);

    public virtual Task<HttpResponseMessage> GetAsync(
        string? methodName,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Get, methodName, null, autoThrowException, cancellationToken);

    public virtual Task<HttpResponseMessage> GetAsync(
        string? methodName,
        Dictionary<string, string> data,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
        => GetAsync(GetUrl(methodName, data), autoThrowException, cancellationToken);

    public virtual Task<TResponse?> GetAsync<TResponse>(
        string? methodName,
        CancellationToken cancellationToken = default)
        => SendAsync<TResponse>(() => CreateRequest(HttpMethod.Get, methodName), cancellationToken);

    public virtual Task<TResponse?> GetAsync<TRequest, TResponse>(
        string? methodName,
        TRequest data,
        CancellationToken cancellationToken = default) where TRequest : class
        => SendAsync<TResponse>(
            () => CreateRequest(HttpMethod.Get, GetUrl(methodName, _typeConvertor.ConvertToKeyValuePairs(data))),
            cancellationToken);

    public virtual Task<TResponse?> GetAsync<TResponse>(
        string? methodName,
        object data,
        CancellationToken cancellationToken = default)
        => SendAsync<TResponse>(
            () => CreateRequest(HttpMethod.Get, GetUrl(methodName, _typeConvertor.ConvertToKeyValuePairs(data))),
            cancellationToken);

    public virtual Task<TResponse?> GetAsync<TResponse>(
        string? methodName,
        Dictionary<string, string> data,
        CancellationToken cancellationToken = default)
        => SendAsync<TResponse>(CreateRequest(HttpMethod.Get, GetUrl(methodName, data)), cancellationToken);

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
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Post, methodName, content, autoThrowException, cancellationToken);

    public virtual Task<HttpResponseMessage> PostAsync<TRequest>(
        string? methodName,
        TRequest data,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
        => SendAsync(CreateRequest(HttpMethod.Post, methodName, data), autoThrowException, cancellationToken);

    public virtual Task<TResponse?> PostAsync<TRequest, TResponse>(
        string? methodName,
        TRequest data,
        CancellationToken cancellationToken = default)
        => SendAsync<TResponse>(CreateRequest(HttpMethod.Post, methodName, data), cancellationToken);

    public virtual Task<TResponse?> PostAsync<TResponse>(
        string? methodName,
        object data,
        CancellationToken cancellationToken = default)
        => PostAsync<object, TResponse>(methodName, data, cancellationToken);

    public virtual Task<HttpResponseMessage> PatchAsync(
        string? methodName,
        HttpContent? content,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Patch, methodName, content, autoThrowException, cancellationToken);

    public virtual Task<HttpResponseMessage> PatchAsync<TRequest>(
        string? methodName,
        TRequest data,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
        => SendAsync(CreateRequest(HttpMethod.Patch, methodName, data), autoThrowException, cancellationToken);

    public virtual Task<TResponse?> PatchAsync<TRequest, TResponse>(
        string? methodName,
        TRequest data,
        CancellationToken cancellationToken = default)
        => SendAsync<TResponse>(() => CreateRequest(HttpMethod.Patch, methodName, data), cancellationToken);

    public virtual Task<TResponse?> PatchAsync<TResponse>(
        string? methodName,
        object data,
        CancellationToken cancellationToken = default)
        => PatchAsync<object, TResponse>(methodName, data, cancellationToken);

    public virtual Task<HttpResponseMessage> PutAsync(
        string? methodName,
        HttpContent? content,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Put, methodName, content, autoThrowException, cancellationToken);

    public virtual Task<HttpResponseMessage> PutAsync<TRequest>(
        string? methodName,
        TRequest data,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
        => SendAsync(() => CreateRequest(HttpMethod.Put, methodName, data), autoThrowException, cancellationToken);

    public virtual Task<TResponse?> PutAsync<TRequest, TResponse>(
        string? methodName,
        TRequest data,
        CancellationToken cancellationToken = default)
        => SendAsync<TResponse>(() => CreateRequest(HttpMethod.Put, methodName, data), cancellationToken);

    public virtual Task<TResponse?> PutAsync<TResponse>(
        string? methodName,
        object data,
        CancellationToken cancellationToken = default)
        => PutAsync<object, TResponse>(methodName, data, cancellationToken);

    public virtual Task<HttpResponseMessage> DeleteAsync(
        string? methodName,
        HttpContent? content,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Delete, methodName, content, autoThrowException, cancellationToken);

    public virtual Task<HttpResponseMessage> DeleteAsync<TRequest>(
        string? methodName,
        TRequest data,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
        => SendAsync(() => CreateRequest(HttpMethod.Delete, methodName, data), autoThrowException, cancellationToken);

    public virtual Task<TResponse?> DeleteAsync<TRequest, TResponse>(
        string? methodName,
        TRequest data,
        CancellationToken cancellationToken = default)
        => SendAsync<TResponse>(() => CreateRequest(HttpMethod.Delete, methodName, data), cancellationToken);

    public virtual Task<TResponse?> DeleteAsync<TResponse>(
        string? methodName,
        object data,
        CancellationToken cancellationToken = default)
        => DeleteAsync<object, TResponse>(methodName, data, cancellationToken);

}
