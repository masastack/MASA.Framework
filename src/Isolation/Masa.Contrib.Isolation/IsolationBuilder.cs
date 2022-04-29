// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation;

public class IsolationBuilder : IIsolationBuilder
{
    public IServiceCollection Services { get; }

    public IsolationBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
