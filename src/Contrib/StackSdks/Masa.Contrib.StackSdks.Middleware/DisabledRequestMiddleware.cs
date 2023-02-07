// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Middleware;

public class DisabledRequestMiddleware : IMiddleware
{
    private readonly IDisabledRequestDeterminer _disabledRequestDeterminer;

    public DisabledRequestMiddleware(IDisabledRequestDeterminer disabledRequestDeterminer)
    {
        _disabledRequestDeterminer = disabledRequestDeterminer;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (_disabledRequestDeterminer.Determiner())
        {
            var endpoint = context.GetEndpoint();
            var disabledRouteAttribute = context.GetEndpoint()?.Metadata
                .GetMetadata<DisabledRouteAttribute>();
            if (disabledRouteAttribute != null)
            {
                //context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                throw new UserFriendlyException("FORBIDDEN");
            }
        }

        return next(context);
    }
}
