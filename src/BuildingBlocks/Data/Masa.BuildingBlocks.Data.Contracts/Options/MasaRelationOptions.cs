// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

public class MasaRelationOptions
{
    public string Name { get; protected set; }
}

public class MasaRelationOptions<TService> : MasaRelationOptions
    where TService : class
{
    public Func<IServiceProvider, TService> Func { get; set; }

    public MasaRelationOptions(string name) => Name = name;

    public MasaRelationOptions(string name, Func<IServiceProvider, TService> func) : this(name)
    {
        Func = func;
    }

    public MasaRelationOptions(string name, Func<IServiceProvider, TService> func)
        : this(name)
    {
        Func = func;
    }
}
