// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.AspNetCore.Mvc;

[AttributeUsage(AttributeTargets.Method)]
public class IncludeMappingAttribute : Attribute
{
    public string? Pattern { get; set; }

    public string? Method { get; set; }

    public IncludeMappingAttribute(string? method = null)
    {
        Method = method;
    }
}
