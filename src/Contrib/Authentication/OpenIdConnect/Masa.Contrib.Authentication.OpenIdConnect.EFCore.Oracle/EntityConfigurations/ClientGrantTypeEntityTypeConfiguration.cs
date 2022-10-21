// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Oracle.EntityConfigurations;

public class ClientGrantTypeEntityTypeConfiguration : IEntityTypeConfiguration<ClientGrantType>
{
    public void Configure(EntityTypeBuilder<ClientGrantType> builder)
    {
        builder.Property(x => x.GrantType).HasMaxLength(250).IsRequired();
    }
}
