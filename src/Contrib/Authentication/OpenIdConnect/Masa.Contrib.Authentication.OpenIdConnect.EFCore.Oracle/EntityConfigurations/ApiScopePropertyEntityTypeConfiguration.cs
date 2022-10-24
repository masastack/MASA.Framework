// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Oracle.EntityConfigurations;

public class ApiScopePropertyEntityTypeConfiguration : IEntityTypeConfiguration<ApiScopeProperty>
{
    public void Configure(EntityTypeBuilder<ApiScopeProperty> builder)
    {
        builder.Property(x => x.Key).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(2000).IsRequired();
    }
}
