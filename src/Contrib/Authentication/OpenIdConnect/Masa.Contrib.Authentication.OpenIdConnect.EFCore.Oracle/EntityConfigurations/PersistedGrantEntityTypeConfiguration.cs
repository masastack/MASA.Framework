// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Oracle.EntityConfigurations;

public class PersistedGrantEntityTypeConfiguration : IEntityTypeConfiguration<PersistedGrant>
{
    public void Configure(EntityTypeBuilder<PersistedGrant> builder)
    {
        builder.Property(x => x.Key).HasMaxLength(200).ValueGeneratedNever();
        builder.Property(x => x.Type).HasMaxLength(50).IsRequired();
        builder.Property(x => x.SubjectId).HasMaxLength(200);
        builder.Property(x => x.SessionId).HasMaxLength(100);
        builder.Property(x => x.ClientId).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(200);
        builder.Property(x => x.CreationTime).IsRequired();
        // 50000 chosen to be explicit to allow enough size to avoid truncation, yet stay beneath the MySql row size limit of ~65K
        // apparently anything over 4K converts to nvarchar(max) on SqlServer
        builder.Property(x => x.Data).HasMaxLength(50000).IsRequired();

        builder.HasKey(x => x.Key);

        builder.HasIndex(x => new { x.SubjectId, x.ClientId, x.Type });
        builder.HasIndex(x => new { x.SubjectId, x.SessionId, x.Type });
        builder.HasIndex(x => x.Expiration);
    }
}
