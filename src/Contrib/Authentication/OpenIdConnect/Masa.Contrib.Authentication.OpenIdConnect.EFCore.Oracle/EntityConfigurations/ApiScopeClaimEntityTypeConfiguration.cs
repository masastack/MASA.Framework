﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Oracle.EntityConfigurations;

public class ApiScopeClaimEntityTypeConfiguration : IEntityTypeConfiguration<ApiScopeClaim>
{
    public void Configure(EntityTypeBuilder<ApiScopeClaim> builder)
    {
        builder.HasKey(apiScopeClaim => apiScopeClaim.Id);

        builder.HasOne(x => x.UserClaim).WithMany().HasForeignKey(x => x.UserClaimId)
            .HasConstraintName("FK_ApiScopeClaim_UserClaimId").IsRequired().OnDelete(DeleteBehavior.Cascade);
    }
}
