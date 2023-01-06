// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public abstract class DefaultResponseMessage : IResponseMessage
{
    protected ILogger<DefaultResponseMessage>? Logger { get; }

    protected CallerFactoryOptions Options { get; }

    protected DefaultResponseMessage(IOptions<CallerFactoryOptions> options, ILogger<DefaultResponseMessage>? logger = null)
    {
        Options = options.Value;
        Logger = logger;
    }

    public virtual async Task<TResponse?> ProcessResponseAsync<TResponse>(HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        await ProcessResponseAsync(response, cancellationToken);

        switch (response.StatusCode)
        {
            case HttpStatusCode.Accepted:
            case HttpStatusCode.NoContent:
                return default;
            default:
                return await FormatResponseAsync<TResponse>(response, cancellationToken);
        }
    }

    public virtual async Task ProcessResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (!response.IsSuccessStatusCode)
        {
            await ProcessResponseExceptionAsync(response, cancellationToken);
            return;
        }

        switch (response.StatusCode)
        {
            case (HttpStatusCode)MasaHttpStatusCode.UserFriendlyException:
                throw new UserFriendlyException(await response.Content.ReadAsStringAsync(cancellationToken));
            case (HttpStatusCode)MasaHttpStatusCode.ValidatorException:
                throw new MasaValidatorException(await response.Content.ReadAsStringAsync(cancellationToken));
            default:
                await ProcessCustomException(response);
                return;
        }
    }

    public virtual Task ProcessCustomException(HttpResponseMessage response) => Task.CompletedTask;

    public async Task ProcessResponseExceptionAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (response.Content.Headers.ContentLength is > 0)
            throw new MasaException(await response.Content.ReadAsStringAsync(cancellationToken));

        throw new MasaException($"ReasonPhrase: {response.ReasonPhrase ?? string.Empty}, StatusCode: {response.StatusCode}");
    }

    private Task<TResponse?> FormatResponseAsync<TResponse>(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        var responseType = typeof(TResponse);
        if (responseType == typeof(Guid) || responseType == typeof(Guid?))
            return FormatResponseByGuidAsync<TResponse>(response, cancellationToken);

        if (responseType == typeof(DateTime) || responseType == typeof(DateTime?))
            return FormatResponseByDateTimeAsync<TResponse>(response, cancellationToken);

        var actualType = Nullable.GetUnderlyingType(responseType);

        if (responseType.GetInterfaces().Any(type => type == typeof(IConvertible)) ||
            (actualType != null && actualType.GetInterfaces().Any(type => type == typeof(IConvertible))))
        {
            return FormatResponseByValueTypeAsync<TResponse>(responseType, actualType, response, cancellationToken);
        }

        return FormatResponseAsync<TResponse>(response.Content, cancellationToken);
    }

    protected virtual async Task<TResponse?> FormatResponseAsync<TResponse>(
        HttpContent httpContent,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await httpContent.ReadFromJsonAsync<TResponse>(
                Options.JsonSerializerOptions ?? MasaApp.GetJsonSerializerOptions()
                , cancellationToken);
        }
        catch (Exception exception)
        {
            Logger?.LogWarning(exception, "{Message}", exception.Message);
            ExceptionDispatchInfo.Capture(exception).Throw();
            return default; //This will never be executed, the previous line has already thrown an exception
        }
    }

    private static async Task<TResponse?> FormatResponseByGuidAsync<TResponse>(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        var content = (await response.Content.ReadAsStringAsync(cancellationToken)).Replace("\"", "");
        if (IsNullOrEmpty(content))
            return default;

        return (TResponse?)(object)Guid.Parse(content);
    }

    private static async Task<TResponse?> FormatResponseByDateTimeAsync<TResponse>(HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        var content = (await response.Content.ReadAsStringAsync(cancellationToken)).Replace("\"", "");
        if (IsNullOrEmpty(content))
            return default;

        return (TResponse?)(object)DateTime.Parse(content);
    }

    private static async Task<TResponse?> FormatResponseByValueTypeAsync<TResponse>(
        Type responseType,
        Type? actualType,
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (IsNullOrEmpty(content))
            return default;

        if (actualType != null)
            return (TResponse?)Convert.ChangeType(content, actualType);

        return (TResponse?)Convert.ChangeType(content, responseType);
    }

    private static bool IsNullOrEmpty(string value) => string.IsNullOrEmpty(value) || value == "null";
}
