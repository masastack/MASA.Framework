// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18N;

public static class Constant
{
    public const string SUPPORTED_CULTURES_NAME = "supportedCultures.json";

    public static readonly string DefaultResourcePath;

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
