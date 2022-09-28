// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Novell.Directory.Ldap;

public static class LdapAttributeSetExtensions
{
    public static void AddAttribute(this LdapAttributeSet ldapAttributes, string name, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            ldapAttributes.Add(new LdapAttribute(name, value));
        }
    }

    public static string GetString(this LdapAttributeSet ldapAttributes, string name)
    {
        ldapAttributes.TryGetValue(name, out var value);
        return value?.StringValue ?? "";
    }

    public static string[] GetStringArray(this LdapAttributeSet ldapAttributes, string name)
    {
        ldapAttributes.TryGetValue(name, out var value);
        return value?.StringValueArray ?? new string[] { };
    }
}
