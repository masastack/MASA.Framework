// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddI18NForBlazor(this IServiceCollection services,
        Action<CultureSettings>? action = null)
    {
        services.TryAddTransient(typeof(II18N<>), typeof(Masa.Contrib.Globalization.I18N.Blazor.I18N<>));
        services.AddI18N(action);
        return services;
    }
}
