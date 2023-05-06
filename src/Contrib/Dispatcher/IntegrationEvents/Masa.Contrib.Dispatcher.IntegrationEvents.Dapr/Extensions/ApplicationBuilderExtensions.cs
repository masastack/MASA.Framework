// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.AspNetCore.Builder;

[ExcludeFromCodeCoverage]
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// If Isolation is used, the subscriber needs to use MasaCloudEvent, otherwise the correct data will not be obtained
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseMasaCloudEvents(this IApplicationBuilder builder)
    {
        MasaArgumentException.ThrowIfNull(builder);

        builder.UseMiddleware<MasaCloudEventsMiddleware>();
        return builder;
    }
}
