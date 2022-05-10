// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster.Tests.Requests.Orders;

public class OrderMultiRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public List<OrderItemRequest> OrderItems { get; set; } = default!;
}
