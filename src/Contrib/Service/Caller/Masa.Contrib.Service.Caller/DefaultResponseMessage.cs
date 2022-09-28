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
        var responseType = typeof(TResponse);
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
                    if (responseType == typeof(Guid) || responseType == typeof(Guid?))
                    {
                        var content = (await response.Content.ReadAsStringAsync(cancellationToken)).Replace("\"", "");
                        if (IsNullOrEmpty(content))
                            return default;

                        return (TResponse?)(object)Guid.Parse(content);
                    }
                    if (responseType == typeof(DateTime) || responseType == typeof(DateTime?))
                    {
                        var content = (await response.Content.ReadAsStringAsync(cancellationToken)).Replace("\"", "");
                        if (IsNullOrEmpty(content))
                            return default;

                        return (TResponse?)(object)DateTime.Parse(content);
                    }

                    var actualType = Nullable.GetUnderlyingType(responseType);

                    if (responseType.GetInterfaces().Any(type => type == typeof(IConvertible)) ||
                        (actualType != null && actualType.GetInterfaces().Any(type => type == typeof(IConvertible))))
                    {
                        var content = await response.Content.ReadAsStringAsync(cancellationToken);
                        if (IsNullOrEmpty(content))
                            return default;

                        if (actualType != null)
                            return (TResponse?)Convert.ChangeType(content, actualType);

                        return (TResponse?)Convert.ChangeType(content, responseType);
                    }

                    try
                    {
                        return await response.Content.ReadFromJsonAsync<TResponse>(
                            _options.JsonSerializerOptions?? MasaApp.GetJsonSerializerOptions()
                            , cancellationToken);
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

    private static bool IsNullOrEmpty(string value) => string.IsNullOrEmpty(value) || value == "null";
}
