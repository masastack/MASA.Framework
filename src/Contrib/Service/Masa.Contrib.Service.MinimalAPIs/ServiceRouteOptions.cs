// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs;

public class ServiceRouteOptions
{
    public bool? DisableAutoMapRoute { get; set; }

    /// <summary>
    /// The prefix, the default is null
    /// Formatter is $"{Prefix}/{Version}/{ServiceName}", any one IsNullOrWhiteSpace would be ignored.
    /// </summary>
    public string? Prefix { get; set; }

    /// <summary>
    /// The default is Null
    /// Formatter is $"{Prefix}/{Version}/{ServiceName}", any one IsNullOrWhiteSpace would be ignored.
    /// </summary>
    public string? Version { get; set; }

    public bool? AutoAppendId { get; set; }

    public bool? PluralizeServiceName { get; set; }

    public List<string>? GetPrefixes { get; set; }

    public List<string>? PostPrefixes { get; set; }

    public List<string>? PutPrefixes { get; set; }

    public List<string>? DeletePrefixes { get; set; }

    /// <summary>
    /// Disable removing the request method prefix that matches the method name
    /// </summary>
    public bool? DisableTrimMethodPrefix { get; set; }

    /// <summary>
    /// After matching request type by prefix fails
    /// Use the request type when matching the request method based on the prefix fails
    /// When the collection is empty, the default Post, Get, Put, Delete all support access
    /// </summary>
    public string[] MapHttpMethodsForUnmatched { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Enable access to public properties
    /// default: false
    /// </summary>
    public bool? EnableProperty { get; set; }
}
