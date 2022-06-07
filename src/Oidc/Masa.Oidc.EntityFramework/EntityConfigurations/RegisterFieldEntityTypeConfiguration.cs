// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.EntityFramework.EntityConfigurations;

public class RegisterFieldEntityTypeConfiguration : IEntityTypeConfiguration<RegisterField>
{
    public void Configure(EntityTypeBuilder<RegisterField> builder)
    {
        builder.HasKey(registerField => registerField.Id);
    }
}
