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

    public string[]? GetPrefixes { get; set; }

    public string[]? PostPrefixes { get; set; }

    public string[]? PutPrefixes { get; set; }

    public string[]? DeletePrefixes { get; set; }
}
