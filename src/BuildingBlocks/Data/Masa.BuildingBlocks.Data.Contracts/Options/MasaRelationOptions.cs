// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

using System;

namespace Masa.BuildingBlocks.Data;

public interface IMasaRelationOptions
{
    public string Name { get; }
}

public class MasaRelationOptions : IMasaRelationOptions
{
    public MasaRelationOptions()
    {
    }

    public string Name { get; protected set; }
}

public interface IMasaRelationOptions<out TService> : IMasaRelationOptions
{
    TService GetService(IServiceProvider serviceProvider);
}

public class MasaRelationOptions<TService> : MasaRelationOptions, IMasaRelationOptions<TService>
    where TService : class
{
    public Func<IServiceProvider, TService> Func { get; set; }

    public MasaRelationOptions(string name) => Name = name;

    public MasaRelationOptions(string name, Func<IServiceProvider, TService> func) : this(name)
    {
        Func = func;
    }

    public TService GetService(IServiceProvider serviceProvider)
    {
        return Func?.Invoke(serviceProvider);
    }
}
