// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Repository.EFCore.IntegrationTests;

public class CustomDispatcherOptions : IDomainEventOptions
{
    public IServiceCollection Services { get; }
    public Assembly[] Assemblies { get; }

    public CustomDispatcherOptions(IServiceCollection services, Assembly[] assemblies)
    {
        Services = services;
        Assemblies = assemblies;
    }
}
