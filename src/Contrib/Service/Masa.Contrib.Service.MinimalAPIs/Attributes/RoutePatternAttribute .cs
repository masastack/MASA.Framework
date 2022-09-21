// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.AspNetCore.Mvc;

[AttributeUsage(AttributeTargets.Method)]
public class RoutePatternAttribute : Attribute
{
    public string? Pattern { get; set; }

    /// <summary>
    /// The request method, the default is null (the request method is automatically identified according to the method name prefix)
    /// </summary>
    public string? HttpMethod { get; set; }

    public bool StartWithBaseUri { get; set; }

    public RoutePatternAttribute(string? pattern = null, bool startWithBaseUri = false)
    {
        Pattern = pattern;
        StartWithBaseUri = startWithBaseUri;
    }
}
