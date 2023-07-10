// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth;

public class EnvironmentMiddleware : ICallerMiddleware
{
    public async Task HandleAsync(MasaHttpContext masaHttpContext, CallerHandlerDelegate next, CancellationToken cancellationToken = default)
    {

        try
        {
            if (masaHttpContext.RequestMessage.Content != null && masaHttpContext.RequestMessage.Content.Headers.ContentType?.MediaType == "application/json")
            {
                var body = await masaHttpContext.RequestMessage.Content.ReadFromJsonAsync<EnvironmentModel>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }, cancellationToken: CancellationToken.None);
                if (!string.IsNullOrEmpty(body?.Environment))
                {
                    masaHttpContext.RequestMessage.Headers.Add(IsolationConsts.ENVIRONMENT, body?.Environment);
                }
            }
        }
        finally
        {
            await next();
        }
    }
}
