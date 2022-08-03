// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster.Tests.Domain.Aggregates.Orders;

public class Order
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public decimal TotalPrice { get; set; }

    public List<OrderItem> OrderItems { get; set; }

    private Order()
    {
        Id = Guid.NewGuid();
    }

    public Order(string name) : this()
    {
        Name = name;
    }

    public Order(string name, OrderItem orderItem) : this(name, new List<OrderItem> { orderItem })
    {
    }

    public Order(string name, List<OrderItem> orderItems) : this(name)
    {
        Name = name;
        OrderItems = orderItems;
        TotalPrice = orderItems.Sum(item => item.Price * item.Number);
    }
}
