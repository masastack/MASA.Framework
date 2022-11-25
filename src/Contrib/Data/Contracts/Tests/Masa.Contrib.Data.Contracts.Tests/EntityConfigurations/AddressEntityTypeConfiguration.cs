// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Contracts.Tests.EntityConfigurations;

public class AddressEntityTypeConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.OwnsOne(address => address.LastLog, address =>
        {
            address.Property(log => log.Level).HasColumnName("level").IsRequired();
            address.Property(log => log.Message).HasColumnName("message").IsRequired();
            address.Property(log => log.CreateTime).HasColumnName("create_time").IsRequired();
        });
    }
}
