// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.EntityFramework.EntityConfigurations;

public class ClientClaimEntityTypeConfiguration : IEntityTypeConfiguration<ClientClaim>
{
    public void Configure(EntityTypeBuilder<ClientClaim> builder)
    {
        builder.Property(x => x.Type).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(250).IsRequired();
    }
}
