// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Oracle.EntityConfigurations;

public class ClientClaimEntityTypeConfiguration : IEntityTypeConfiguration<ClientClaim>
{
    public void Configure(EntityTypeBuilder<ClientClaim> builder)
    {
        builder.Property(x => x.Type).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(250).IsRequired();
    }
}
