// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18n;

[Serializable]
public class I18nResourceDictionary : Dictionary<Type, I18nResource>
{
    public I18nResourceDictionary()
    {
    }

    protected I18nResourceDictionary(SerializationInfo info, StreamingContext context) : base(info, context){}

    public I18nResource Add<TResource>(params Type[] baseResourceTypes)
    {
        return Add(typeof(TResource), baseResourceTypes);
    }

    public I18nResource Add(Type resourceType, params Type[] baseResourceTypes)
    {
        List<Type> types = new List<Type>(baseResourceTypes);

        var customAttributes = resourceType.GetCustomAttributes(typeof(InheritResourceAttribute), true).FirstOrDefault();
        if (customAttributes is InheritResourceAttribute inheritResourceAttribute)
        {
            types.AddRange(inheritResourceAttribute.ResourceTypes);
        }

        return this[resourceType] = new I18nResource(resourceType, types.Distinct());
    }

    public bool TryAdd<TResource>(Action<I18nResource> action, params Type[] baseResourceTypes)
    {
        return TryAdd(typeof(TResource), action, baseResourceTypes);
    }

    public bool TryAdd(Type resourceType, Action<I18nResource> action, params Type[] baseResourceTypes)
    {
        if (this.ContainsKey(resourceType))
            return false;

        var i18nResource = Add(resourceType, baseResourceTypes);
        action.Invoke(i18nResource);
        return true;
    }

    public Task<bool> TryAddAsync<TResource>(Func<I18nResource, Task> func, params Type[] baseResourceTypes)
    {
        return TryAddAsync(typeof(TResource), func, baseResourceTypes);
    }

    public async Task<bool> TryAddAsync(Type resourceType, Func<I18nResource, Task> func, params Type[] baseResourceTypes)
    {
        if (this.ContainsKey(resourceType))
            return false;

        var i18nResource = Add(resourceType, baseResourceTypes);
        await func.Invoke(i18nResource);
        return true;
    }

    public I18nResource? GetOrNull(Type resourceType)
    {
        if (!ContainsKey(resourceType))
            return null;

        return this[resourceType];
    }

    public I18nResource? GetOrNull<TResource>() => GetOrNull(typeof(TResource));
}
