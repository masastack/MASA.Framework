// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Localization;

[Serializable]
public class LocalizationResourceDictionary : Dictionary<Type, LocalizationResource>
{
    public LocalizationResource Add<TResource>()
    {
        return Add(typeof(TResource));
    }

    public LocalizationResource Add(Type resourceType)
    {
        if (ContainsKey(resourceType))
            throw new ArgumentException("This resource is already added before: " + resourceType.FullName);

        return this[resourceType] = new LocalizationResource(resourceType);
    }

    public LocalizationResource? GetOrNull(Type resourceType)
    {
        if (!ContainsKey(resourceType))
            return null;

        return this[resourceType];
    }

    public LocalizationResource? GetOrNull<TResource>() => GetOrNull(typeof(TResource));
}
