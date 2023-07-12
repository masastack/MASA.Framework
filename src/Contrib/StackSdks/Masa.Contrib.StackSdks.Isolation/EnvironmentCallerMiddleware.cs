// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Isolation;

public class EnvironmentCallerMiddleware : ICallerMiddleware
{
    readonly ILogger<EnvironmentCallerMiddleware>? _logger;

    public EnvironmentCallerMiddleware(ILoggerFactory? loggerFactory = null)
    {
        _logger = loggerFactory?.CreateLogger<EnvironmentCallerMiddleware>();
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
                if (!string.IsNullOrEmpty(obj?.Environment) && !masaHttpContext.RequestMessage.Headers.Any(x => x.Key == IsolationConsts.ENVIRONMENT))
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

