// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs;

public class ServiceBaseOptions
{
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

    public bool? PluralizeServiceName { get; set; }

    public string[]? GetPrefixs { get; set; }

    public string[]? PostPrefixs { get; set; }

    public string[]? PutPrefixs { get; set; }

    public string[]? DeletePrefixs { get; set; }
}
