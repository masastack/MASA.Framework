// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller;

internal class DefaultCallerFactory : AbstractMasaFactory<ICaller, CallerFactoryOptions, CallerRelationOptions>, ICallerFactory
{
    protected override string DefaultServiceNotFoundMessage => "No default Caller found, you may need service.AddCaller()";
    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{name}] Caller, it was not found";

    public DefaultCallerFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
