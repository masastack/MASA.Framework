// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public class CallerMiddlewareFactoryOptions : MasaFactoryOptions<CallerMiddlewareRelationOptions>
{
    public void AddMiddleware(string name, Func<IServiceProvider, ICallerMiddleware> implementationFactory)
    {
        var option = Options.FirstOrDefault(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (option != null) option.AddMiddlewareFunc(implementationFactory);
        else Options.Add(new CallerMiddlewareRelationOptions(name)
        {
            Middlewares = { implementationFactory }
        });
    }
}
