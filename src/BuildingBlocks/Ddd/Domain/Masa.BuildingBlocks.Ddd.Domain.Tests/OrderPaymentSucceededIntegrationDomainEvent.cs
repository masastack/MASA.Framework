// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Tests;

public record OrderPaymentSucceededIntegrationDomainEvent : IntegrationDomainEvent
{
    public Guid OrderId { get; set; }
}
