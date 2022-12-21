// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Enums.Tests.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class ENameAttribute : Attribute
{
    public string Name { get; set; }

    public ENameAttribute() : this(string.Empty)
    {
    }

    public ENameAttribute(string name)
    {
        Name = name;
    }
}
