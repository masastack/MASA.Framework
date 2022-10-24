// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Oracle.EntityConfigurations;

public class ClientRedirectUriEntityTypeConfiguration : IEntityTypeConfiguration<ClientRedirectUri>
{
    public void Configure(EntityTypeBuilder<ClientRedirectUri> builder)
    {
        builder.Property(x => x.RedirectUri).HasMaxLength(2000).IsRequired();
    }
}
