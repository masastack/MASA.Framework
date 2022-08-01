// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Ldap.Novell;

public class LdapOptions
{
    public string ServerAddress { get; set; } = null!;

    public int ServerPort { get; set; }

    public int ServerPortSsl { get; set; }

    public string BaseDn { get; set; } = null!;

    private string _userSearchBaseDn = string.Empty;

    public string UserSearchBaseDn
    {
        get
        {
            if (string.IsNullOrEmpty(_userSearchBaseDn))
            {
                return BaseDn;
            }
            return _userSearchBaseDn;
        }
        set { _userSearchBaseDn = value; }
    }

    private string _groupSearchBaseDn = string.Empty;

    public string GroupSearchBaseDn
    {
        get
        {
            if (string.IsNullOrEmpty(_groupSearchBaseDn))
            {
                return BaseDn;
            }
            return _groupSearchBaseDn;
        }
        set { _groupSearchBaseDn = value; }
    }

    public string RootUserDn { get; set; } = null!;

    public string RootUserPassword { get; set; } = null!;
}
