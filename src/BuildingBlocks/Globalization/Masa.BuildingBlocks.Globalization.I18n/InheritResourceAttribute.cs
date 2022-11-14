// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18n;

[AttributeUsage(AttributeTargets.Class)]
public class InheritResourceAttribute : Attribute
{
    public Type[] ResourceTypes { get; }

    public InheritResourceAttribute(params Type[] resourceTypes)
    {
        ResourceTypes = resourceTypes;
    }
}
