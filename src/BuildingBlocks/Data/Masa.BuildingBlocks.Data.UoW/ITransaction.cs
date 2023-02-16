// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.UoW;

public interface ITransaction
{
    [NotMapped]
    [JsonIgnore]
    IUnitOfWork? UnitOfWork { get; set; }
}
