// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Caller;

public class DefaultAuthenticationServiceFactory :
    MasaFactoryBase<IAuthenticationService, AuthenticationServiceRelationOptions>,
    IAuthenticationServiceFactory
{
    protected override string DefaultServiceNotFoundMessage => "No default AuthenticationService found, you may need service.AddCaller()";

    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{0}] AuthenticationService, it was not found";

    protected override MasaFactoryOptions<AuthenticationServiceRelationOptions> FactoryOptions => _options.Value;

    private readonly IOptions<AuthenticationServiceFactoryOptions> _options;

    public DefaultAuthenticationServiceFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _options = serviceProvider.GetRequiredService<IOptions<AuthenticationServiceFactoryOptions>>();
    }
}
