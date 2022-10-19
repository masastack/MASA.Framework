// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18N;

[Serializable]
public class I18NResourceDictionary : Dictionary<Type, I18NResource>
{
    public I18NResource Add<TResource>()
    {
        return Add(typeof(TResource));
    }

    public I18NResource Add(Type resourceType)
    {
        return this[resourceType] = new I18NResource(resourceType);
    }

    public I18NResource? GetOrNull(Type resourceType)
    {
        if (!ContainsKey(resourceType))
            return null;

        return this[resourceType];
    }

    public I18NResource? GetOrNull<TResource>() => GetOrNull(typeof(TResource));
}
