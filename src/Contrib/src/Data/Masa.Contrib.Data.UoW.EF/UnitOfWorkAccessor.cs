// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.UoW.EF;

public class UnitOfWorkAccessor : IUnitOfWorkAccessor
{
    public MasaDbContextConfigurationOptions? CurrentDbContextOptions { get; set; }
}
