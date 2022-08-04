// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.DistributedLock.Medallion;

public class MedallionBuilder
{
    public IServiceCollection Services { get; }

    public MedallionBuilder(IServiceCollection services) => Services = services;
}
