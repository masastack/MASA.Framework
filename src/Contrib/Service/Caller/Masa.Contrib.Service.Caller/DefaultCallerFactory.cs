// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller;

internal class DefaultCallerFactory : MasaFactoryBase<ICaller, CallerRelationOptions>, ICallerFactory
{
    protected override string DefaultServiceNotFoundMessage => "No default Caller found, you may need service.AddCaller()";

    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{0}] Caller, it was not found";

    protected override MasaFactoryOptions<CallerRelationOptions> FactoryOptions => _optionsMonitor.CurrentValue;

    private readonly IOptionsMonitor<CallerFactoryOptions> _optionsMonitor;

    public DefaultCallerFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<CallerFactoryOptions>>();
    }
}
