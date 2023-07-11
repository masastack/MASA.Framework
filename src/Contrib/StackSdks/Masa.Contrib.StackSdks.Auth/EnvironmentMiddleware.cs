// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.Logging;

namespace Masa.Contrib.StackSdks.Auth;

public class EnvironmentMiddleware : ICallerMiddleware
{
    readonly ILogger<EnvironmentMiddleware>? _logger;

    public EnvironmentMiddleware(ILoggerFactory? loggerFactory = null)
    {
        _logger = loggerFactory?.CreateLogger<EnvironmentMiddleware>();
    }

    public async Task HandleAsync(MasaHttpContext masaHttpContext, CallerHandlerDelegate next, CancellationToken cancellationToken = default)
    {

        try
        {
            if (masaHttpContext.RequestMessage.Content != null && masaHttpContext.RequestMessage.Content.Headers.ContentType?.MediaType == "application/json")
            {
                var body = await masaHttpContext.RequestMessage.Content.ReadAsStringAsync(CancellationToken.None);
                var obj = JsonSerializer.Deserialize<EnvironmentModel>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (!string.IsNullOrEmpty(obj?.Environment))
                {
                    masaHttpContext.RequestMessage.Headers.Add(IsolationConsts.ENVIRONMENT, obj?.Environment);
                }
            }
        }
        catch (Exception exception)
        {
            _logger?.LogDebug(exception, "Environment Middleware Deserialize!");
        }
        finally
        {
            await next();
        }
    }
}
