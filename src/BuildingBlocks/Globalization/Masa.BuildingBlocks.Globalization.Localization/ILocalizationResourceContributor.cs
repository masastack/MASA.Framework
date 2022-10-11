// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.Localization;

/// <summary>
/// 具体资源类下具体语言的资源
/// </summary>
public interface ILocalizationResourceContributor
{
    LocalizedString? GetOrNull(string name);

    void ResetResource();
}
