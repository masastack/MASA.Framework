// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Oidc.EntityFramework.EntityConfigurations;

public class ClientPostLogoutRedirectUriEntityTypeConfiguration : IEntityTypeConfiguration<ClientPostLogoutRedirectUri>
{
    public void Configure(EntityTypeBuilder<ClientPostLogoutRedirectUri> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
