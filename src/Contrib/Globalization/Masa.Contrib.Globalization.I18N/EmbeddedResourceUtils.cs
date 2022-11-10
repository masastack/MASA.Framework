// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18N;

public class EmbeddedResourceUtils
{
    private readonly List<KeyValuePair<Assembly, string[]>> _list;

    public EmbeddedResourceUtils(params Assembly[] assemblies) : this(assemblies.ToList())
    {
    }

    public EmbeddedResourceUtils(IEnumerable<Assembly> assemblies)
    {
        _list = assemblies
            .Select(assembly => new KeyValuePair<Assembly, string[]>(assembly, assembly.GetManifestResourceNames()))
            .ToList();
    }

    public List<KeyValuePair<Assembly, string[]>> GetResources(string resourcesDirectory)
    {
        var list = new List<KeyValuePair<Assembly, string[]>>();
        foreach (var item in _list)
        {
            var data = item.Value.Where(resourceName
                => resourceName.Contains(FormatResourcesDirectory(resourcesDirectory), StringComparison.OrdinalIgnoreCase)).ToArray();
            if (data.Length > 0)
            {
                list.Add(new KeyValuePair<Assembly, string[]>(item.Key, data));
            }
        }
        return list;
    }

    public static Stream? GetStream(Assembly assembly, string fileName) => assembly.GetManifestResourceStream(fileName);

    public static string? GetCulture(string resourcesDirectory, string fileName)
    {
        var formatResourcesDirectory = FormatResourcesDirectory(resourcesDirectory);
        var index = fileName.IndexOf(formatResourcesDirectory, StringComparison.OrdinalIgnoreCase);
        if (index >= 0)
            return fileName.Substring(index).Replace(formatResourcesDirectory + ".", "").Replace(".json", "");

        return null;
    }

    private static string FormatResourcesDirectory(string resourcesDirectory)
    {
        return resourcesDirectory.Replace(Path.DirectorySeparatorChar.ToString(), ".");
    }
}
