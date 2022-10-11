﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.Localization;

public class LocalizationResourceDictionary : Dictionary<Type, LocalizationResource>
{
    public LocalizationResource Add<TResource>(string? defaultCultureName = null)
    {
        return Add(typeof(TResource), defaultCultureName);
    }

    public LocalizationResource Add(Type resourceType, string? defaultCultureName = null)
    {
        if (ContainsKey(resourceType))
            throw new Exception("This resource is already added before: " + resourceType.FullName);

        return this[resourceType] = new LocalizationResource(resourceType, defaultCultureName);
    }

    public LocalizationResource Get<TResource>()
    {
        var resourceType = typeof(TResource);

        if (!ContainsKey(resourceType))
            throw new Exception("Can not find a resource with given type: " + resourceType.FullName);

        return this[resourceType];
    }
}
