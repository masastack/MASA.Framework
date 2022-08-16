// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class DefaultDeserializerFactory : AbstractMasaFactory<IDeserializer, DeserializerFactoryOptions, DeserializerRelationOptions>,
    IDeserializerFactory
{
    protected override string DefaultServiceNotFoundMessage => "Default deserializer not found, you need to add it, like services.AddJson()";

    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{name}] deserializer, it was not found";

    public DefaultDeserializerFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
