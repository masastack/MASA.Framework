// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs;

public class Url
{
    /// <summary>
    /// The prefix, the default is null, consistent with the global prefix, if it is string.Empty, the Prefix is ignored
    /// </summary>
    public string? Prefix { get; set; }

    /// <summary>
    /// The default is Null, which is consistent with the global Version. If it is String.Empty, the Version is ignored.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// service name, defaults to null
    /// </summary>
    public string? ServiceName { get; set; }

    public string ToString(Type type)
    {
        var list = new List<string>()
        {
            Prefix ?? MasaService.Prefix,
            Version ?? MasaService.Version,
            ServiceName ?? GetServiceName(type, "service")
        };
        return string.Join('/', list.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    private string GetServiceName(Type type, string name)
    {
        var typeName = type.Name.ToLower();
        var index = typeName.LastIndexOf(name, StringComparison.Ordinal);
        return typeName.Remove(index);
    }
}
