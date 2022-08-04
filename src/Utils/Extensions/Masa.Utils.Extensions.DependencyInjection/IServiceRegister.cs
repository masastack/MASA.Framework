// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public interface IServiceRegister
{
    void Add(IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime lifetime);
}
