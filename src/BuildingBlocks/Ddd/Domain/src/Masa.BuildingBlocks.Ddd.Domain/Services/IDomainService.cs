// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Services;

public interface IDomainService
{
    IDomainEventBus EventBus { get; }
}
