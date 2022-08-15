// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.TypeConverts;

public class TypeConvertFactoryOptions : MasaFactoryOptions<TypeConvertRelationOptions>
{
    public TypeConvertFactoryOptions TryMapping(Func<IServiceProvider, ITypeConvertProvider> func)
        => TryMapping(Microsoft.Extensions.Options.Options.DefaultName, func);

    public TypeConvertFactoryOptions TryMapping(string name, Func<IServiceProvider, ITypeConvertProvider> func)
    {
        if (Options.Any(opt => opt.Name == name.ToLower()))
            return this;

        Options.Add(new TypeConvertRelationOptions(name.ToLower(), func));
        return this;
    }

    public TypeConvertFactoryOptions Mapping(Func<IServiceProvider, ITypeConvertProvider> func)
        => Mapping(Microsoft.Extensions.Options.Options.DefaultName, func);

    public TypeConvertFactoryOptions Mapping(string name, Func<IServiceProvider, ITypeConvertProvider> func)
    {
        var builder = Options.FirstOrDefault(opt => opt.Name == name.ToLower());
        if (builder != null)
        {
            builder.Func = func;
        }
        else
        {
            Options.Add(new TypeConvertRelationOptions(name.ToLower(), func));
        }
        return this;
    }

    public Func<IServiceProvider, ITypeConvertProvider>? GetTypeConvert()
        => GetTypeConvert(Microsoft.Extensions.Options.Options.DefaultName);

    public Func<IServiceProvider, ITypeConvertProvider>? GetTypeConvert(string name)
    {
        return Options.FirstOrDefault(opt => opt.Name == name.ToLower())?.Func;
    }
}
