// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.TypeConverts;

public class TypeConvertOptions
{
    private List<TypeConvertOptionsBuilder> _mappings { get; set; } = new();

    public Func<IServiceProvider, ITypeConvertProvider>? GetTypeConvert(string? name = null)
    {
        if (name == null)
        {
            var builder = _mappings.FirstOrDefault(b => b.IsDefault == true) ?? _mappings.FirstOrDefault();
            return builder?.Func;
        }
        else
        {

            var builder = _mappings.FirstOrDefault(b => b.Name == name.ToLower());
            return builder?.Func;
        }
    }

    public TypeConvertOptions Mapping(string name, Func<IServiceProvider, ITypeConvertProvider> func, bool? isDefault = null)
    {
        var builder = _mappings.FirstOrDefault(b => b.Name == name.ToLower());
        if (builder != null)
        {
            builder.Func = func;
            builder.IsDefault = isDefault;
        }
        else
        {
            _mappings.Add(new TypeConvertOptionsBuilder(name.ToLower(), func, isDefault));
        }
        return this;
    }
}
