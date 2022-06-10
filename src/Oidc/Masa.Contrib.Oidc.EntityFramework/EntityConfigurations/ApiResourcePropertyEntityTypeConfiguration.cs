// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Oidc.EntityFramework.EntityConfigurations;

public class ApiResourcePropertyEntityTypeConfiguration : IEntityTypeConfiguration<ApiResourceProperty>
{
    public void Configure(EntityTypeBuilder<ApiResourceProperty> builder)
    {
        builder.Property(x => x.Key).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(2000).IsRequired();
    }
}
