// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller;

internal class DefaultCallerFactory : ICallerFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<CallerRelationOptions> _callers;

    public DefaultCallerFactory(IServiceProvider serviceProvider, IOptions<CallerFactoryOptions> options)
    {
        _serviceProvider = serviceProvider;
        _callers = options.Value.Options;
    }

    public ICaller Create()
    {
        var caller = _callers.SingleOrDefault(c => c.Name == Options.DefaultName) ?? _callers.FirstOrDefault()!;
        return caller.Func.Invoke(_serviceProvider);
    }

    public ICaller Create(string name)
    {
        var caller = _callers.SingleOrDefault(c => c.Name == name);
        if (caller == null)
            throw new NotSupportedException($"Please make sure you have used [{name}] Caller");

        return caller.Func.Invoke(_serviceProvider);
    }
}
