// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.EntityFramework.EntityConfigurations;

public class IdentityResourcePropertyEntityTypeConfiguration : IEntityTypeConfiguration<IdentityResourceProperty>
{
    public void Configure(EntityTypeBuilder<IdentityResourceProperty> builder)
    {
        builder.Property(x => x.Key).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(2000).IsRequired();
    }
}
