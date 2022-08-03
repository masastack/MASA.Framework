// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Ldap.Novell.Entries;

public class LdapAddress
{
    public string Street { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string StateName { get; set; } = string.Empty;

    public string CountryName { get; set; } = string.Empty;

    public string CountryCode { get; set; } = string.Empty;
}
