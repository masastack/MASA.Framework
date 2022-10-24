// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Oracle.EntityConfigurations;

public class IdentityResourceClaimEntityTypeConfiguration : IEntityTypeConfiguration<IdentityResourceClaim>
{
    public void Configure(EntityTypeBuilder<IdentityResourceClaim> builder)
    {
        builder.HasKey(identityResourceClaim => identityResourceClaim.Id);
        builder.HasOne(x => x.UserClaim).WithMany().HasForeignKey(x => x.UserClaimId)
            .HasConstraintName("FK_IdResClaim_UserClaimId").IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.IdentityResourceId).HasDatabaseName("IX_IdResourceClaim_ResourceId");
        builder.HasIndex(x => x.UserClaimId).HasDatabaseName("IX_IdResourceClaim_UserClaimId");
    }
}
