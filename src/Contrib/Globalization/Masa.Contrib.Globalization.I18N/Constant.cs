// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18N;

public static class Constant
{
    public const string SUPPORTED_CULTURES_NAME = "supportedCultures.json";

    public static readonly string DefaultResourcePath
        = Path.Combine("Resources", "I18N");

    internal static readonly string DefaultFrameworkResourcePath
        = Path.Combine(DefaultResourcePath, "Framework");

    internal static readonly string DefaultFrameworkParameterValidationResourcePath =
        Path.Combine(DefaultFrameworkResourcePath, "ParameterValidations");

    internal static readonly string DefaultFrameworkLanguageResourcePath
        = Path.Combine(DefaultFrameworkResourcePath, "Languages");
}
