// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Ldap.Novell.Entries;

public class LdapUser
{
    /// <summary>
    /// ObjectSID contains the value for the Security Identifier (SID) of the entry.
    /// </summary>
    public string ObjectSid { get; set; } = string.Empty;

    /// <summary>
    /// ObjectGUID is an Attribute-Names which represents a Universally Unique Identifier as used in Microsoft Active Directory.
    /// </summary>
    public string ObjectGuid { get; set; } = string.Empty;

    public string ObjectCategory { get; set; } = string.Empty;

    public string ObjectClass { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string CommonName { get; set; } = string.Empty;

    public string DistinguishedName { get; set; } = string.Empty;

    public string SamAccountName { get; set; } = string.Empty;

    public int SamAccountType { get; set; }

    public string[] MemberOf { get; set; } = Array.Empty<string>();

    public bool IsDomainAdmin { get; set; }

    public string UserPrincipalName { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    public string EmailAddress { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public LdapAddress Address { get; set; } = new();
}
