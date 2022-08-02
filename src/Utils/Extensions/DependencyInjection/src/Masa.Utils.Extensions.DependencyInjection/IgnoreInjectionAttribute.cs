// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class IgnoreInjectionAttribute : Attribute
{
    public bool Cascade { get; set; }

    public IgnoreInjectionAttribute(bool cascade = false)
    {
        Cascade = cascade;
    }
}
