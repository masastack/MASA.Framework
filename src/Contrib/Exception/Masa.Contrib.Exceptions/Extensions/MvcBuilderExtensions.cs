// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class MvcBuilderExtensions
{
    public static IMvcBuilder AddMasaExceptionHandler(this IMvcBuilder builder)
    {
        return builder.AddMasaExceptionHandler(_ =>
        {
        });
    }

    public static IMvcBuilder AddMasaExceptionHandler(this IMvcBuilder builder, Action<MasaExceptionHandlerOptions> action)
    {
        builder.Services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add<MvcGlobalExceptionFilter>();
        });

        builder.Services.Configure(action);

        return builder;
    }
}
