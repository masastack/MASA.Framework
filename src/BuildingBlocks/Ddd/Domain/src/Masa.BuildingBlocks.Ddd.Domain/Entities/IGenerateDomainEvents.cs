// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Entities;

public interface IGenerateDomainEvents
{
    IEnumerable<IDomainEvent> GetDomainEvents();

    void ClearDomainEvents();
}
