// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.Localization;

public class LocalizationResource
{
    /// <summary>
    /// 语言、内容集合
    /// </summary>
    private Dictionary<string, ILocalizationResourceContributor> _dictionary { get; }

    public Type ResourceType { get; }

    public string? DefaultCultureName { get; }

    // /// <summary>
    // /// 用于获取父类的key、value
    // /// </summary>
    // public List<Type> BaseResourceTypes { get; set; }

    public LocalizationResource(Type resourceType, string? defaultCultureName)
    {
        _dictionary = new();
        ResourceType = resourceType;
        DefaultCultureName = defaultCultureName;
        // BaseResourceTypes = new();
    }
}
