// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class LdapOptionsModel
{
    public string ServerAddress { get; set; }

    public int ServerPort { get; set; }

    public int ServerPortSsl { get; set; }

    public string BaseDn { get; set; }

    public string UserSearchBaseDn { get; set; }

    public string GroupSearchBaseDn { get; set; }

    public string RootUserDn { get; set; }

    public string RootUserPassword { get; set; }

    public LdapOptionsModel(string serverAddress, int serverPort, string baseDn, string userSearchBaseDn, string groupSearchBaseDn, string rootUserDn, string rootUserPassword)
    {
        ServerAddress = serverAddress;
        ServerPort = serverPort;
        BaseDn = baseDn;
        UserSearchBaseDn = userSearchBaseDn;
        GroupSearchBaseDn = groupSearchBaseDn;
        RootUserDn = rootUserDn;
        RootUserPassword = rootUserPassword;
    }
}
