// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data;

public class DefaultTypeConvertFactory : ITypeConvertFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TypeConvertOptions _options;

    public DefaultTypeConvertFactory(IOptions<TypeConvertOptions> options, IServiceProvider serviceProvider)
    {
        _options = options.Value;
        _serviceProvider = serviceProvider;
    }

    public ITypeConvertProvider Create()
    {
        var func = _options.GetTypeConvert();
        if (func == null) throw new NotImplementedException("TypeConvert provider get failed");

        return func.Invoke(_serviceProvider);
    }

    public ITypeConvertProvider Create(string name)
    {
        var func = _options.GetTypeConvert(name);
        if (func == null) throw new NotImplementedException("TypeConvert provider get failed");

        return func.Invoke(_serviceProvider);
    }
}
