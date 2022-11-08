// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Globalization.I18N.BlazorWebAssembly;

internal static class Constant
{
    internal const string SUPPORTED_CULTURES_NAME = "supportedCultures.json";

    internal static readonly string DefaultResourcePath;

    internal static readonly string DefaultFrameworkResourcePath;

    internal static readonly string DefaultFrameworkExceptionResourcePath;

    internal static readonly string DefaultFrameworkLanguageResourcePath;

    static Constant()
    {
        DefaultResourcePath = Path.Combine("Resources", "I18n");
        DefaultFrameworkResourcePath = Path.Combine(DefaultResourcePath, "Framework");
        DefaultFrameworkExceptionResourcePath = Path.Combine(DefaultFrameworkResourcePath, "Exceptions");
        DefaultFrameworkLanguageResourcePath = Path.Combine(DefaultFrameworkResourcePath, "Languages");
    }
}
