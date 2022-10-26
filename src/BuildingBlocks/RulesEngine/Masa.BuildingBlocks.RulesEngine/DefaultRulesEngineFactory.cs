// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.RulesEngine;

public class DefaultRulesEngineFactory : MasaFactoryBase<IRulesEngineClient, RulesEngineRelationOptions>,
    IRulesEngineFactory
{
    protected override string DefaultServiceNotFoundMessage
        => $"No default {nameof(IRulesEngineClient)} found, you may need services.AddRulesEngine(options => options.UseRulesEngine())";

    protected override string SpecifyServiceNotFoundMessage
        => $"Please make sure you have used [{0}] {nameof(IRulesEngineClient)}, it was not found";

    protected override MasaFactoryOptions<RulesEngineRelationOptions> FactoryOptions => _options.Value;

    private readonly IOptions<RulesEngineFactoryOptions> _options;

    public DefaultRulesEngineFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _options = serviceProvider.GetRequiredService<IOptions<RulesEngineFactoryOptions>>();
    }
}
