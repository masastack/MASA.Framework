// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Repository.EFCore.Tests.Domain.Entities;

public class Orders : AuditAggregateRoot<int, Guid>
{
    public int OrderNumber { get; set; }

    public DateTime OrderDate { get; private set; }

    public string OrderStatus { get; private set; }

    public string Description { get; set; } = default!;

    public List<OrderItem> OrderItems { get; set; }

    public Orders(int id)
    {
        this.OrderDate = DateTime.UtcNow;
        this.OrderItems = new();
        this.OrderStatus = "Submitted";
        base.Id = id;
    }

    /// <summary>
    /// Joint primary key, when this method does not exist, the primary key is Id
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<(string Name, object Value)> GetKeys()
    {
        return new List<(string Name, object value)>
        {
            ("Id", Id),
            ("OrderNumber", OrderNumber)
        };
    }
}
