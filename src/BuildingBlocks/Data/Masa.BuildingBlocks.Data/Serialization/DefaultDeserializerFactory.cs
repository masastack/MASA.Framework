﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class DefaultDeserializerFactory : MasaFactoryBase<IDeserializer, DeserializerRelationOptions>,
    IDeserializerFactory
{
    protected override string DefaultServiceNotFoundMessage => "Default deserializer not found, you need to add it, like services.AddJson()";

    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{0}] deserializer, it was not found";
    protected override MasaFactoryOptions<DeserializerRelationOptions> FactoryOptions => _optionsMonitor.CurrentValue;

    private readonly IOptionsMonitor<DeserializerFactoryOptions> _optionsMonitor;

    public DefaultDeserializerFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<DeserializerFactoryOptions>>();
    }
}
