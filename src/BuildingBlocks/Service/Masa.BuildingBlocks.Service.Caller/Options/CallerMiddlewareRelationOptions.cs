// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public class CallerMiddlewareRelationOptions : MasaRelationOptions
{
    public List<Func<IServiceProvider, ICallerMiddleware>> Funcs { get; }

    public CallerMiddlewareRelationOptions(string name)
    {
        Name = name;
        Funcs = new List<Func<IServiceProvider, ICallerMiddleware>>();
    }

    public void AddMiddlewareFunc(Func<IServiceProvider, ICallerMiddleware> func)
    {
        Funcs.Add(func);
    }
}
