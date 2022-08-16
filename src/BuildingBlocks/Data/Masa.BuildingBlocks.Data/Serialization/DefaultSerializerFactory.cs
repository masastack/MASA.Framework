// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class DefaultSerializerFactory : AbstractMasaFactory<ISerializer, SerializerFactoryOptions, SerializerRelationOptions>,
    ISerializerFactory
{
    protected override string DefaultServiceNotFoundMessage => "Default serializer not found, you need to add it, like services.AddJson()";

    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{name}] serializer, it was not found";

    public DefaultSerializerFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
