﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.Tests;

public class DefaultXmlResponseMessage : IResponseMessage
{
    private readonly ILogger<DefaultResponseMessage>? _logger;

    public DefaultXmlResponseMessage(ILogger<DefaultResponseMessage>? logger = null)
    {
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
                        var content = await response.Content.ReadAsStringAsync(cancellationToken);
                        if (string.IsNullOrEmpty(content))
                            return (TResponse)(object?)null!;

                        return (TResponse?)(object)Guid.Parse(content.Replace("\"", ""));
                    }
                    if (typeof(TResponse) == typeof(DateTime) || typeof(TResponse) == typeof(DateTime?))
                    {
                        var content = await response.Content.ReadAsStringAsync(cancellationToken);
                        if (string.IsNullOrEmpty(content))
                            return (TResponse)(object?)null!;

                        return (TResponse?)(object)DateTime.Parse(content.Replace("\"", ""));
                    }
                    if (typeof(TResponse).GetInterfaces().Any(type => type == typeof(IConvertible)))
                    {
                        var content = await response.Content.ReadAsStringAsync(cancellationToken);
                        return (TResponse)Convert.ChangeType(content, typeof(TResponse));
                    }
                    try
                    {
                        var res = await response.Content.ReadAsStringAsync(cancellationToken);
                        return XmlUtils.Deserialize<TResponse>(res) ??
                            throw new ArgumentException("The response cannot be empty or there is an error in deserialization");
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
            throw new Exception(await response.Content.ReadAsStringAsync(cancellationToken));

        throw new MasaException($"ReasonPhrase: {response.ReasonPhrase ?? string.Empty}, StatusCode: {response.StatusCode}");
    }
}
