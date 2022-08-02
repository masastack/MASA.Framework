// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Oidc.Models.Constans;

public class StandardIdentityResources
{
    [Description("Your user identifier")]
    public static IdentityResourceModel OpenId = new()
    {
        Name = "openid",
        DisplayName = "Your user identifier",
        Description = "Your user identifier",
        Required = true,
        UserClaims = new List<string>()
        {
            StandardUserClaims.Subject
        }
    };

    [Description("Your address")]
    public static IdentityResourceModel Address = new()
    {
        Name = "address",
        DisplayName = "Your address",
        Description = "Your address",
        Emphasize = true,
        UserClaims = new List<string>()
        {
            StandardUserClaims.Address
        }
    };

    [Description("Your email address")]
    public static IdentityResourceModel Email = new()
    {
        Name = "email",
        DisplayName = "Your email address",
        Description = "Your email address",
        Emphasize = true,
        UserClaims = new List<string>()
        {
            StandardUserClaims.Email,
            StandardUserClaims.EmailVerified,
        }
    };

    [Description("Your phone number")]
    public static IdentityResourceModel Phone = new()
    {
        Name = "phone",
        DisplayName = "Your phone number",
        Description = "Your phone number",
        Emphasize = true,
        UserClaims = new List<string>()
        {
            StandardUserClaims.PhoneNumber,
            StandardUserClaims.PhoneNumberVerified
        }
    };

    [Description("User profile")]
    public static IdentityResourceModel Profile = new()
    {
        Name = "profile",
        DisplayName = "User profile",
        Description = "Your user profile information (first name, last name, etc.)",
        Emphasize = true,
        UserClaims = new List<string>()
        {
            StandardUserClaims.Name,
            StandardUserClaims.FamilyName,
            StandardUserClaims.GivenName,
            StandardUserClaims.MiddleName,
            StandardUserClaims.NickName,
            StandardUserClaims.PreferredUserName,
            StandardUserClaims.Profile,
            StandardUserClaims.Picture,
            StandardUserClaims.WebSite,
            StandardUserClaims.Gender,
            StandardUserClaims.BirthDate,
            StandardUserClaims.ZoneInfo,
            StandardUserClaims.Locale,
            StandardUserClaims.UpdatedAt
        }
    };

    static List<IdentityResourceModel>? _identityResources;

    public static List<IdentityResourceModel> IdentityResources => _identityResources ??= GetIdentityResources();

    static List<IdentityResourceModel> GetIdentityResources()
    {
        var identityResources = new List<IdentityResourceModel>();
        var fields = typeof(StandardIdentityResources).GetFields(BindingFlags.Static | BindingFlags.Public);
        foreach (var field in fields)
        {
            var idrs = (IdentityResourceModel)(field.GetValue(null) ?? throw new Exception("Error standard identity resources data"));
            identityResources.Add(idrs);
        }

        return identityResources;
    }
}
