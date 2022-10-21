// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Oracle.EntityConfigurations;

public class DeviceFlowCodesEntityTypeConfiguration : IEntityTypeConfiguration<DeviceFlowCodes>
{
    public void Configure(EntityTypeBuilder<DeviceFlowCodes> builder)
    {
        builder.Property(x => x.DeviceCode).HasMaxLength(200).IsRequired();
        builder.Property(x => x.UserCode).HasMaxLength(200).IsRequired();
        builder.Property(x => x.SubjectId).HasMaxLength(200);
        builder.Property(x => x.SessionId).HasMaxLength(100);
        builder.Property(x => x.ClientId).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(200);
        builder.Property(x => x.CreationTime).IsRequired();
        builder.Property(x => x.Expiration).IsRequired();
        // 50000 chosen to be explicit to allow enough size to avoid truncation, yet stay beneath the MySql row size limit of ~65K
        // apparently anything over 4K converts to nvarchar(max) on SqlServer
        builder.Property(x => x.Data).HasMaxLength(50000).IsRequired();

        builder.HasKey(x => new { x.UserCode });

        builder.HasIndex(x => x.DeviceCode).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(x => x.Expiration);
    }
}
