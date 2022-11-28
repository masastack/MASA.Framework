// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Repository.EFCore.IntegrationTests.EntityTypeConfigurations;

public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Orders>
{
    public void Configure(EntityTypeBuilder<Orders> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id);
        builder.OwnsOne(o => o.ReceiveAddress, o =>
        {
            o.Property(a => a.Street);
            o.Property(a => a.City);
            o.Property(a => a.State);
            o.OwnsOne(a => a.Country, c =>
            {
                c.Property<string>(t => t.Name);
            });
            o.Property(a => a.ZipCode);
        });
        builder.HasMany(t => t.OrderItems)
            .WithOne(t => t.Orders)
            .HasForeignKey(o => o.OrderId);
    }
}
