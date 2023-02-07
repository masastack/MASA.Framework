// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Builder;

namespace Masa.Contrib.StackSdks.Middleware;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAddStackMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<DisabledRequestMiddleware>();
        return app;
    }
}
