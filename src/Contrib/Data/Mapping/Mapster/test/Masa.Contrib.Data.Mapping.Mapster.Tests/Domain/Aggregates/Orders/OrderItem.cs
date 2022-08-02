// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster.Tests.Domain.Aggregates.Orders;

public class OrderItem
{
    public string Name { get; set; }

    public decimal Price { get; set; }

    public int Number { get; set; }

    public OrderItem(string name, decimal price) : this(name, price, 1)
    {

    }

    public OrderItem(string name, decimal price, int number)
    {
        Name = name;
        Price = price;
        Number = number;
    }
}
