// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.UoW;

public interface IUnitOfWorkAccessor
{
    /// <summary>
    /// Only exists after the DbContext is confirmed to be created
    /// </summary>
    MasaDbContextConfigurationOptions CurrentDbContextOptions { get; set; }
}
