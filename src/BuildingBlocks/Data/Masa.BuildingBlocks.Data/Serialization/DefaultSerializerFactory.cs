// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class DefaultSerializerFactory : ISerializerFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<SerializerFactoryOptions> _serializerFactoryOptions;
    private readonly SerializerRelationOptions? _defaultSerializerOptions;

    public DefaultSerializerFactory(IServiceProvider serviceProvider, IOptions<SerializerFactoryOptions> serializerFactoryOptions)
    {
        _serviceProvider = serviceProvider;
        _serializerFactoryOptions = serializerFactoryOptions;
        _defaultSerializerOptions = serializerFactoryOptions.Value.Options.FirstOrDefault(options
                => options.Name == Options.DefaultName) ??
            serializerFactoryOptions.Value.Options.FirstOrDefault();
    }

    public ISerializer Create()
    {
        if (_defaultSerializerOptions == null)
            throw new NotImplementedException("Default serializer not found, you need to add it, like services.AddJson()");

        return _defaultSerializerOptions.Func.Invoke(_serviceProvider);
    }

    public ISerializer Create(string name)
    {
        var serializerOptions =
            _serializerFactoryOptions.Value.Options.FirstOrDefault(options
                => options.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (serializerOptions == null)
            throw new NotImplementedException($"No serializer found for 【{name}】");

        return serializerOptions.Func.Invoke(_serviceProvider);
    }
}
