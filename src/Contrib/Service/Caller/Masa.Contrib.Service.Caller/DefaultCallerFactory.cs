// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller;

internal class DefaultCallerFactory : ICallerFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<CallerRelationOptions> _options;

    public DefaultCallerFactory(IServiceProvider serviceProvider, IOptions<CallerFactoryOptions> options)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value.Options;
    }

    public ICaller Create()
    {
        var options = _options.SingleOrDefault(c => c.Name == Options.DefaultName) ?? _options.FirstOrDefault()!;
        return options.Func.Invoke(_serviceProvider);
    }

    public ICaller Create(string name)
    {
        var options = _options.SingleOrDefault(c => c.Name == name);
        if (options == null)
            throw new NotSupportedException($"Please make sure you have used [{name}] Caller");

        return options.Func.Invoke(_serviceProvider);
    }
}
