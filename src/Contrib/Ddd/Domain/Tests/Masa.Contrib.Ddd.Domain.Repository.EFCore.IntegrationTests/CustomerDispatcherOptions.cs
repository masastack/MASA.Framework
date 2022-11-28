// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Repository.EFCore.IntegrationTests;

public class CustomerDispatcherOptions : IDispatcherOptions
{
    public IServiceCollection Services { get; }
    public Assembly[] Assemblies { get; }

    public CustomerDispatcherOptions(IServiceCollection services, Assembly[] assemblies)
    {
        Services = services;
        Assemblies = assemblies;
    }
}
