// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller;

public class DefaultResponseMessage : IResponseMessage
{
    private readonly ILogger<DefaultResponseMessage>? _logger;
    private readonly CallerFactoryOptions _options;

    public DefaultResponseMessage(IOptions<CallerFactoryOptions> options, ILogger<DefaultResponseMessage>? logger = null)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<TResponse?> ProcessResponseAsync<TResponse>(HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        if (response.IsSuccessStatusCode)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.Accepted:
                case HttpStatusCode.NoContent:
                    return default;
                case (HttpStatusCode)MasaHttpStatusCode.UserFriendlyException:
                    throw new UserFriendlyException(await response.Content.ReadAsStringAsync(cancellationToken));
                default:
                    if (typeof(TResponse) == typeof(Guid) || typeof(TResponse) == typeof(Guid?))
                    {
                        var content = (await response.Content.ReadAsStringAsync(cancellationToken)).Replace("\"", "");
                        if (IsNullOrEmpty(content))
                            return (TResponse)(object?)null!;

                        return (TResponse?)(object)Guid.Parse(content);
                    }
                    if (typeof(TResponse) == typeof(DateTime) || typeof(TResponse) == typeof(DateTime?))
                    {
                        var content = (await response.Content.ReadAsStringAsync(cancellationToken)).Replace("\"", "");
                        if (IsNullOrEmpty(content))
                            return (TResponse)(object?)null!;

                        return (TResponse?)(object)DateTime.Parse(content);
                    }
                    if (typeof(TResponse).GetInterfaces().Any(type => type == typeof(IConvertible)) ||
                        (typeof(TResponse).IsGenericType && typeof(TResponse).GenericTypeArguments.Length == 1 && typeof(TResponse)
                            .GenericTypeArguments[0].GetInterfaces().Any(type => type == typeof(IConvertible))))
                    {
                        var content = await response.Content.ReadAsStringAsync(cancellationToken);
                        if (IsNullOrEmpty(content))
                            return (TResponse)(object?)null!;

                        return (TResponse)Convert.ChangeType(content, typeof(TResponse));
                    }
                    try
                    {
                        return await response.Content.ReadFromJsonAsync<TResponse>(_options.JsonSerializerOptions, cancellationToken);
                    }
                    catch (Exception exception)
                    {
                        _logger?.LogWarning(exception, exception.Message);
                        ExceptionDispatchInfo.Capture(exception).Throw();
                        return default; //This will never be executed, the previous line has already thrown an exception
                    }
            }
        }

        await ProcessResponseExceptionAsync(response, cancellationToken);
        return default; //never executed
    }

    public async Task ProcessResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (response.IsSuccessStatusCode)
        {
            switch (response.StatusCode)
            {
                case (HttpStatusCode)MasaHttpStatusCode.UserFriendlyException:
                    throw new UserFriendlyException(await response.Content.ReadAsStringAsync(cancellationToken));
                default:
                    return;
            }
        }

        await ProcessResponseExceptionAsync(response, cancellationToken);
    }

    public async Task ProcessResponseExceptionAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (response.Content.Headers.ContentLength is > 0)
            throw new MasaException(await response.Content.ReadAsStringAsync(cancellationToken));

        throw new MasaException($"ReasonPhrase: {response.ReasonPhrase ?? string.Empty}, StatusCode: {response.StatusCode}");
    }

    private bool IsNullOrEmpty(string value) => string.IsNullOrEmpty(value) || value == "null";
}
