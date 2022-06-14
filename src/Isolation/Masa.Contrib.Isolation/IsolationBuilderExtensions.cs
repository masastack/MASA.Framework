// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.AspNetCore.Builder;

public static class IsolationBuilderExtensions
{
    public static TApplicationBuilder UseIsolation<TApplicationBuilder>(this TApplicationBuilder app) where TApplicationBuilder : IApplicationBuilder
    {
        app.UseMiddleware<IsolationMiddleware>();
        return app;
    }
}
