// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Identity.IdentityModel;

public class DefaultTypeConvertProvider : ITypeConvertProvider
{
    public T ConvertTo<T>(string value)
    {
        if (typeof(T) == typeof(Guid))
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(value)!;

        return (T)Convert.ChangeType(value, typeof(T));
    }
}
