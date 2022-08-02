// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

public static class EnumExtensions
{
    public static DescriptionAttribute GetDescription(this Enum enumSubitem)
    {
        string value = enumSubitem.ToString();

        var fieldInfo = enumSubitem.GetType().GetField(value);

        if (fieldInfo != null)
        {
            var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes == null || attributes.Length == 0)
            {
                return new DescriptionAttribute(value);
            }
            else
            {
                return (DescriptionAttribute)attributes[0];
            }
        }
        else
        {
            return new DescriptionAttribute();
        }
    }

    public static T? GetAttribute<T>(this Enum enumSubitem)
        where T : Attribute, new()
        => EnumUtil.GetSubitemAttribute<T>(enumSubitem);
}
