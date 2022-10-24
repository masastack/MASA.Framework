// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Oracle.EntityConfigurations;

public class ApiResourceClaimEntityTypeConfiguration : IEntityTypeConfiguration<ApiResourceClaim>
{
    public void Configure(EntityTypeBuilder<ApiResourceClaim> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.UserClaim).WithMany().HasForeignKey(x => x.UserClaimId)
            .HasConstraintName("FK_ApiResClaim_UserClaimId").IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.ApiResourceId).HasDatabaseName("IX_ApiResClaim_ApiResourceId");
        builder.HasIndex(x => x.UserClaimId).HasDatabaseName("IX_ApiResClaim_UserClaimId");
    }
}
