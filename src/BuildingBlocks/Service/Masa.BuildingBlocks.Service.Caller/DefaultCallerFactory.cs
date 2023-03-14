// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Caller;

internal class DefaultCallerFactory : MasaFactoryBase<ICaller, CallerRelationOptions>, ICallerFactory
{
    protected override string DefaultServiceNotFoundMessage => "No default Caller found, you may need service.AddCaller()";

    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{0}] Caller, it was not found";

    protected override MasaFactoryOptions<CallerRelationOptions> FactoryOptions => _options.Value;

    private readonly IOptions<CallerFactoryOptions> _options;

    public DefaultCallerFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _options = serviceProvider.GetRequiredService<IOptions<CallerFactoryOptions>>();
    }

    protected override IServiceProvider GetServiceProvider(string name)
    {
        var lifetime = FactoryOptions.Options.Where(opt => opt.Name == name).Select(opt => opt.Lifetime).FirstOrDefault();
        switch (lifetime)
        {
            case ServiceLifetime.Scoped:
            case null when TransientServiceProvider.GetService<IAuthenticationServiceFactory>()?.TryCreate(name, out _) ?? false:
                return TransientServiceProvider.GetRequiredService<ScopedService>().ServiceProvider;
            case ServiceLifetime.Transient:
                return TransientServiceProvider;
            case ServiceLifetime.Singleton:
            default:
                return TransientServiceProvider.GetRequiredService<SingletonService>().ServiceProvider;
        }
    }
}
