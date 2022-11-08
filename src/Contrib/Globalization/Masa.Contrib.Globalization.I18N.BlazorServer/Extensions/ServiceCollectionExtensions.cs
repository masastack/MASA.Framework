// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddI18NForBlazorServer(this IServiceCollection services, Action<CultureSettings>? action = null)
    {
        services.AddHttpContextAccessor();
        services.TryAddTransient(typeof(II18N<>), typeof(Masa.Contrib.Globalization.I18N.BlazorServer.I18NOfT<>));
        services.TryAddScoped<ILanguageProvider, DefaultLanguageProvider>();
        services.AddI18N(setting =>
        {
            action?.Invoke(setting);

            if (setting.ResourcesDirectory.IsNullOrWhiteSpace())
                setting.ResourcesDirectory = Path.Combine(I18NResourceResourceConfiguration.BaseDirectory, "Resources", "I18n");
        });
        return services;
    }
}
