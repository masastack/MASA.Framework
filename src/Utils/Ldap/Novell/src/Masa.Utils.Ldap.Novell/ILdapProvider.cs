// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Ldap.Novell;

public interface ILdapProvider
{
    Task<LdapEntry?> GetGroupAsync(string groupName);

    IAsyncEnumerable<LdapUser> GetUsersInGroupAsync(string groupName);

    Task<LdapUser?> GetUsersByEmailAddressAsync(string emailAddress);

    Task<LdapUser?> GetUserByUserNameAsync(string userName);

    IAsyncEnumerable<LdapUser> GetAllUserAsync();

    Task<List<LdapEntry>> GetPagingUserAsync(int pageSize);

    Task AddUserAsync(LdapUser user, string password);

    Task DeleteUserAsync(string distinguishedName);

    Task<bool> AuthenticateAsync(string distinguishedName, string password);
}
