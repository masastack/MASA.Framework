// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Transaction;

public class OrderDbContext : MasaDbContext
{
    public DbSet<Order> Order { get; set; }

    public DbSet<OrderStatus> OrderStatus { get; set; }

    public OrderDbContext(MasaDbContextOptions<OrderDbContext> options) : base(options)
    {

    }
}
