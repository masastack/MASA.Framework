// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.AspNetCore.Mvc;

[AttributeUsage(AttributeTargets.Method)]
public class RoutePatternAttribute : Attribute
{
    public string Pattern { get; set; }

    public bool StartWithBaseUri { get; set; }

    public RoutePatternAttribute(string pattern, bool startWithBaseUri = false)
    {
        Pattern = pattern;
        StartWithBaseUri = startWithBaseUri;
    }
}
