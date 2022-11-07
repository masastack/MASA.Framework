// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18N;

[Serializable]
public class I18NResourceDictionary : Dictionary<Type, I18NResource>
{
    public I18NResource Add<TResource>(params Type[] baseResourceTypes)
    {
        return Add(typeof(TResource), baseResourceTypes);
    }

    public I18NResource Add(Type resourceType, params Type[] baseResourceTypes)
    {
        List<Type> types = new List<Type>(baseResourceTypes);

        var customAttributes = resourceType.GetCustomAttributes(typeof(InheritResourceAttribute), true).FirstOrDefault();
        if (customAttributes is InheritResourceAttribute inheritResourceAttribute)
        {
            types.AddRange(inheritResourceAttribute.ResourceTypes);
        }

        return this[resourceType] = new I18NResource(resourceType, types.Distinct());
    }

    public bool TryAdd<TResource>(Action<I18NResource> action, params Type[] baseResourceTypes)
    {
        return TryAdd(typeof(TResource), action, baseResourceTypes);
    }

    public bool TryAdd(Type resourceType, Action<I18NResource> action, params Type[] baseResourceTypes)
    {
        if (this.ContainsKey(resourceType))
            return false;

        var i18NResource = Add(resourceType, baseResourceTypes);
        action.Invoke(i18NResource);
        return true;
    }

    public I18NResource? GetOrNull(Type resourceType)
    {
        if (!ContainsKey(resourceType))
            return null;

        return this[resourceType];
    }

    public I18NResource? GetOrNull<TResource>() => GetOrNull(typeof(TResource));
}
