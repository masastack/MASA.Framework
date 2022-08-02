// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Ldap.Novell;

public class LdapFactory : ILdapFactory
{
    public ILdapProvider CreateProvider(LdapOptions ldapOptions)
    {
        return new LdapProvider(ldapOptions);
    }
}
