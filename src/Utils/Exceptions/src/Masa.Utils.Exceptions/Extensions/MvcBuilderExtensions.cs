// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class MvcBuilderExtensions
{
    public static IMvcBuilder AddMasaExceptionHandler(this IMvcBuilder builder)
    {
        return builder.AddMasaExceptionHandler(_ => { });
    }

    public static IMvcBuilder AddMasaExceptionHandler(this IMvcBuilder builder, Action<MasaExceptionHandlerOptions> action)
    {
        builder.Services.AddLocalization();

        builder.Services.Configure<MvcOptions>(options => { options.Filters.Add<MvcGlobalExcetionFilter>(); });

        builder.Services.Configure(action);

        return builder;
    }
}
