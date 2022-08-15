// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data;

public class DefaultTypeConvertFactory : ITypeConvertFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<TypeConvertFactoryOptions> _typeConvertFactoryOptions;
    private readonly TypeConvertRelationOptions? _defaultOptions;

    public DefaultTypeConvertFactory(IOptions<TypeConvertFactoryOptions> typeConvertFactoryOptions, IServiceProvider serviceProvider)
    {
        _typeConvertFactoryOptions = typeConvertFactoryOptions;
        _defaultOptions = _typeConvertFactoryOptions.Value.Options.FirstOrDefault(options
                => options.Name == Options.DefaultName) ??
            _typeConvertFactoryOptions.Value.Options.FirstOrDefault();
        _serviceProvider = serviceProvider;
    }

    public ITypeConvertProvider Create()
    {
        if (_defaultOptions == null)
            throw new NotImplementedException("Default typeConvert not found, you need to add it");

        return _defaultOptions.Func.Invoke(_serviceProvider);
    }

    public ITypeConvertProvider Create(string name)
    {
        var typeConvertOptions =
            _typeConvertFactoryOptions.Value.Options.FirstOrDefault(options
                => options.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (typeConvertOptions == null)
            throw new NotImplementedException($"No TypeConvert found for 【{name}】");

        return typeConvertOptions.Func.Invoke(_serviceProvider);
    }
}
