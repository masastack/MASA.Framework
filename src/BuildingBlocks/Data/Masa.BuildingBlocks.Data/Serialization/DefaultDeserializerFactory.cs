// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class DefaultDeserializerFactory : IDeserializerFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<DeserializerFactoryOptions> _deserializerFactoryOptions;
    private readonly DeserializerRelationOptions? _defaultDeserializerOptions;

    public DefaultDeserializerFactory(IServiceProvider serviceProvider, IOptions<DeserializerFactoryOptions> deserializerFactoryOptions)
    {
        _serviceProvider = serviceProvider;
        _deserializerFactoryOptions = deserializerFactoryOptions;
        _defaultDeserializerOptions = deserializerFactoryOptions.Value.Options.FirstOrDefault(options
                => options.Name == Options.DefaultName) ??
            deserializerFactoryOptions.Value.Options.FirstOrDefault();
    }

    public IDeserializer Create()
    {
        if (_defaultDeserializerOptions == null)
            throw new NotImplementedException("Default deserializer not found, you need to add it, like services.AddJson()");

        return _defaultDeserializerOptions.Func.Invoke(_serviceProvider);
    }

    public IDeserializer Create(string name)
    {
        var deserializerOptions =
            _deserializerFactoryOptions.Value.Options.FirstOrDefault(options
                => options.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (deserializerOptions == null)
            throw new NotImplementedException($"No deserializer found for 【{name}】");

        return deserializerOptions.Func.Invoke(_serviceProvider);
    }
}
