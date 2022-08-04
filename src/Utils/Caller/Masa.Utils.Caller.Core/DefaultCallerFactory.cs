// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.Core;

internal class DefaultCallerFactory : ICallerFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<CallerRelations> _callers;

    public DefaultCallerFactory(IServiceProvider serviceProvider, CallerOptions options)
    {
        _serviceProvider = serviceProvider;
        _callers = options.Callers;
    }

    public ICallerProvider CreateClient()
    {
        var caller = _callers.SingleOrDefault(c => c.IsDefault) ?? _callers.FirstOrDefault()!;
        return caller.Func.Invoke(_serviceProvider);
    }

    public ICallerProvider CreateClient(string name)
    {
        var caller = _callers.SingleOrDefault(c => c.Name == name);
        if (caller == null)
            throw new NotSupportedException($"Please make sure you have used [{name}] Caller");

        return caller.Func.Invoke(_serviceProvider);
    }
}
