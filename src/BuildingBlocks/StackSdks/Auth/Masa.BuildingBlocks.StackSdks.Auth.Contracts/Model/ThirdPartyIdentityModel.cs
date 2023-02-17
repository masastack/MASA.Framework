// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class ThirdPartyIdentityModel
{
    /// <summary>
    /// Unique identifier, usually the user ID
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// actual name
    /// </summary>
    public string? Name { get; set; }

    public string? NickName { get; set; }

    public string? MiddleName { get; set; }

    public string? FamilyName { get; set; }

    public string? GivenName { get; set; }

    public string? PreferredUserName { get; set; }

    /// <summary>
    /// Basic information
    /// </summary>
    public string? Profile { get; set; }

    /// <summary>
    /// avatar
    /// </summary>
    public string? Picture { get; set; }

    public string? WebSite { get; set; }

    public string? Email { get; set; }

    public string? Gender { get; set; }

    public string? BirthDate { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    /// <summary>
    /// detailed address
    /// </summary>
    public string? Formatted { get; set; }

    public string? StreetAddress { get; set; }

    /// <summary>
    /// city
    /// </summary>
    public string? Locality { get; set; }

    /// <summary>
    /// Province
    /// </summary>
    public string? Region { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public string? Account { get; set; }

    public string? Company { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string Issuer { get; set; } = "";
}
