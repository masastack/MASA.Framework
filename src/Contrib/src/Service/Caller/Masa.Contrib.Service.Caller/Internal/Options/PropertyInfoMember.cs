// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Internal.Options;

internal class PropertyInfoMember
{
    public PropertyInfo Property { get; }

    public string Name { get; }

    public bool NeedSerialize { get; }

    public PropertyInfoMember(PropertyInfo property, string name, bool needSerialize)
    {
        Property = property;
        Name = name;
        NeedSerialize = needSerialize;
    }

    public bool TryGetValue<TRequest>(TRequest data, out string value) where TRequest : class
    {
        value = string.Empty;
        var propertyValue = Property.GetValue(data);
        if (propertyValue == null || (!NeedSerialize && propertyValue.ToString() == null))
            return false;

        value = !NeedSerialize ? propertyValue.ToString()! : JsonSerializer.Serialize(propertyValue);
        return true;
    }
}
