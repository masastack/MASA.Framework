// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class DefaultSerializerFactory : MasaFactoryBase<ISerializer, SerializerRelationOptions>,
    ISerializerFactory
{
    protected override string DefaultServiceNotFoundMessage => "Default serializer not found, you need to add it, like services.AddJson()";

    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{0}] serializer, it was not found";
    protected override MasaFactoryOptions<SerializerRelationOptions> FactoryOptions => _options.Value;

    private readonly IOptionsSnapshot<SerializerFactoryOptions> _options;

    public DefaultSerializerFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _options = serviceProvider.GetRequiredService<IOptionsSnapshot<SerializerFactoryOptions>>();
    }
}
