// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace FluentValidation;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static class GlobalValidationOptions
{
    private static string? _defaultCulture;

    public static void SetDefaultCulture(string culture) => _defaultCulture = culture;

    public static string DefaultCulture => _defaultCulture ?? System.Globalization.CultureInfo.CurrentUICulture.Name;
}
