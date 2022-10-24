// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Oracle.EntityConfigurations;

public class ClientScopeEntityTypeConfiguration : IEntityTypeConfiguration<ClientScope>
{
    public void Configure(EntityTypeBuilder<ClientScope> builder)
    {
        builder.Property(x => x.Scope).HasMaxLength(200).IsRequired();
    }
}
