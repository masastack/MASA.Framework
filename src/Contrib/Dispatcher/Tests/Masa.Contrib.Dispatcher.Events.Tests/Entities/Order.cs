// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;

namespace Masa.Contrib.Dispatcher.Events.Tests.Entities;

public class Order
{
    public int Id { get; set; }

    [MaxLength(2)]
    public string Name { get; set; }

    [ForeignKey("OrderStatusId")]
    public int OrderStatusId { get; set; }

    public OrderStatus OrderStatus { get; set; }
}
