// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Tests.DomainServices;

/// <summary>
/// Non-public DomainService does not support automatic injection
/// </summary>
internal class OrderDomainService : DomainService
{
    private readonly ILogger<OrderDomainService> _logger;

    private OrderDomainService(ILogger<OrderDomainService> logger)
    {
        _logger = logger;
    }
}
