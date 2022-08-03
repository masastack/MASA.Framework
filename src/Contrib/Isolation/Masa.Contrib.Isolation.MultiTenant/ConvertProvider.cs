// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.MultiTenant;

public class ConvertProvider : IConvertProvider
{
    public object ChangeType(string value, Type conversionType)
    {
        var result = conversionType == typeof(Guid) ? Guid.Parse(value) : Convert.ChangeType(value, conversionType);
        return result;
    }
}
