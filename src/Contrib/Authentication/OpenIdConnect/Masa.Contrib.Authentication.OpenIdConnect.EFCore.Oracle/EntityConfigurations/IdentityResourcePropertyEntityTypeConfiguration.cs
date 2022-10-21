// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Oracle.EntityConfigurations;

public class IdentityResourcePropertyEntityTypeConfiguration : IEntityTypeConfiguration<IdentityResourceProperty>
{
    public void Configure(EntityTypeBuilder<IdentityResourceProperty> builder)
    {
        builder.Property(x => x.Key).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(2000).IsRequired();
        builder.HasIndex(x => x.IdentityResourceId).HasDatabaseName("IX_IdResourceProp_ResourceId");
    }
}
