// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.Localization;

public class MasaLocalizationOptions
{
    public Type DefaultResourceType { get; set; }

    public List<LanguageInfo> Languages { get; set; }
}
