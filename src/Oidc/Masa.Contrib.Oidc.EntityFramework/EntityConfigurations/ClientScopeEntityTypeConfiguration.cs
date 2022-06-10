// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Oidc.EntityFramework.EntityConfigurations;

public class ClientScopeEntityTypeConfiguration : IEntityTypeConfiguration<ClientScope>
{
    public void Configure(EntityTypeBuilder<ClientScope> builder)
    {
        builder.Property(x => x.Scope).HasMaxLength(200).IsRequired();
    }
}
