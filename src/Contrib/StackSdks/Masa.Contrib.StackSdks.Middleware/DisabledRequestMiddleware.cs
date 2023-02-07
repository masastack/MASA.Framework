// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Middleware;

internal class DisabledRequestMiddleware : IMiddleware
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
            var disabledRouteAttribute = (endpoint as RouteEndpoint)?.Metadata
                .GetMetadata<DisabledRouteAttribute>();
            if (disabledRouteAttribute != null)
            {
                //throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.DEMO_ENVIRONMENT_FORBIDDEN);
            }
        }

        return next(context);
    }
}
