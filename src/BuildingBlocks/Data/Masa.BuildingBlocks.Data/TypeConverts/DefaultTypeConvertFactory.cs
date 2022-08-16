// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data;

public class DefaultTypeConvertFactory : AbstractMasaFactory<ITypeConvertProvider, TypeConvertRelationOptions>,
    ITypeConvertFactory
{
    protected override string DefaultServiceNotFoundMessage
        => "Default typeConvert not found, you need to add it, like services.AddTypeConvert()";

    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{name}] typeConvert, it was not found";

    protected override MasaFactoryOptions<TypeConvertRelationOptions> FactoryOptions => _optionsMonitor.CurrentValue;

    private readonly IOptionsMonitor<TypeConvertFactoryOptions> _optionsMonitor;

    public DefaultTypeConvertFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<TypeConvertFactoryOptions>>();
    }
}
