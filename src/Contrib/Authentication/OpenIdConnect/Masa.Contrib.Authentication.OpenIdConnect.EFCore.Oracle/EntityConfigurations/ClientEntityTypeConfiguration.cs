// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Oracle.EntityConfigurations;

public class ClientEntityTypeConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.Property(x => x.ClientId).HasMaxLength(400).IsRequired();
        builder.Property(x => x.ProtocolType).HasMaxLength(400).IsRequired();
        builder.Property(x => x.ClientName).HasMaxLength(400);
        builder.Property(x => x.ClientUri).HasMaxLength(2000).IsRequired(false);
        builder.Property(x => x.LogoUri).HasMaxLength(2000).IsRequired(false);
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired(false);
        builder.Property(x => x.FrontChannelLogoutUri).HasMaxLength(2000).IsRequired(false);
        builder.Property(x => x.BackChannelLogoutUri).HasMaxLength(2000).IsRequired(false);
        builder.Property(x => x.ClientClaimsPrefix).HasMaxLength(200).IsRequired(false);
        builder.Property(x => x.PairWiseSubjectSalt).HasMaxLength(200).IsRequired(false);
        builder.Property(x => x.UserCodeType).HasMaxLength(100).IsRequired(false);
        builder.Property(x => x.AllowedIdentityTokenSigningAlgorithms).HasMaxLength(100).IsRequired(false).HasColumnName("AllowedIdTokenSignAlgorithm");
        builder.Property(x => x.FrontChannelLogoutSessionRequired).HasColumnName("FrontChannelLogoutSessionReq");
        builder.Property(x => x.AlwaysIncludeUserClaimsInIdToken).HasColumnName("AlwaysIncUserClaimsInIdToken");
        builder.Property(x => x.BackChannelLogoutSessionRequired).HasColumnName("BackChannelLogoutSessionReq");
        builder.Property(x => x.UpdateAccessTokenClaimsOnRefresh).HasColumnName("UpdateTokenClaimsOnRefresh");
        //builder.HasIndex(x => x.ClientId);
        builder.HasIndex(x => x.ClientId).IsUnique().HasFilter("[IsDeleted] = 0").HasDatabaseName("IX_Client_ClientId");

        builder.HasMany(x => x.AllowedGrantTypes).WithOne(x => x.Client).HasForeignKey(x => x.ClientId)
            .HasConstraintName("FK_ClientGrantType_ClientId").IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.RedirectUris).WithOne(x => x.Client).HasForeignKey(x => x.ClientId)
            .HasConstraintName("FK_ClientRedirectUri_ClientId").IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.PostLogoutRedirectUris).WithOne(x => x.Client).HasForeignKey(x => x.ClientId)
            .HasConstraintName("FK_ClientPostLotRUri_ClientId").IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.AllowedScopes).WithOne(x => x.Client).HasForeignKey(x => x.ClientId)
            .HasConstraintName("FK_ClientScope_ClientId").IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.ClientSecrets).WithOne(x => x.Client).HasForeignKey(x => x.ClientId)
            .HasConstraintName("FK_ClientSecret_ClientId").IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Claims).WithOne(x => x.Client).HasForeignKey(x => x.ClientId)
            .HasConstraintName("FK_ClientClaim_ClientId").IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.IdentityProviderRestrictions).WithOne(x => x.Client).HasForeignKey(x => x.ClientId)
            .HasConstraintName("FK_ClientIdPRest_ClientId").IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.AllowedCorsOrigins).WithOne(x => x.Client).HasForeignKey(x=> x.ClientId)
            .HasConstraintName("FK_ClientCorsOrigin_ClientId").IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Properties).WithOne(x => x.Client).HasForeignKey(x => x.ClientId)
            .HasConstraintName("FK_ClientProperty_ClientId").IsRequired().OnDelete(DeleteBehavior.Cascade);
    }
}
