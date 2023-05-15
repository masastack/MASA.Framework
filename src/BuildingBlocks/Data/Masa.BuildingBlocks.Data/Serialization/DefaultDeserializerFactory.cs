// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

public class DefaultDeserializerFactory : MasaFactoryBase<IDeserializer, MasaRelationOptions<IDeserializer>>,
    IDeserializerFactory
{
    protected override string DefaultServiceNotFoundMessage => "Default deserializer not found, you need to add it, like services.AddJson()";

    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{0}] deserializer, it was not found";
    protected override MasaFactoryOptions<MasaRelationOptions<IDeserializer>> FactoryOptions => _options.CurrentValue;

    private readonly IOptionsMonitor<DeserializerFactoryOptions> _options;

    public DefaultDeserializerFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _options = serviceProvider.GetRequiredService<IOptionsMonitor<DeserializerFactoryOptions>>();
    }
}
