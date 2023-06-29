// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Isolation;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseStackIsolation(this WebApplication app)
    {
        app.UseIsolation();
        app.UseMiddleware<EnvironmentMiddleware>();
        return app;
    }
}
