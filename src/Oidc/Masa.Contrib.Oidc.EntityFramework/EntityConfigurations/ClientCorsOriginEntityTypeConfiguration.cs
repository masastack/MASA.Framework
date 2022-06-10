// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Oidc.EntityFramework.EntityConfigurations;

public class ClientCorsOriginEntityTypeConfiguration : IEntityTypeConfiguration<ClientCorsOrigin>
{
    public void Configure(EntityTypeBuilder<ClientCorsOrigin> builder)
    {
        builder.Property(x => x.Origin).HasMaxLength(150).IsRequired();
    }
}
