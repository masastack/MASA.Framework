// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;

namespace Masa.Utils.Ldap.Novell;

public class LdapProvider : ILdapProvider, IDisposable
{
    private ILdapConnection? _ldapConnection;
    private readonly LdapOptions _ldapOptions;

    private readonly string[] _attributes =
    {
        "objectSid", "objectGUID", "objectCategory", "objectClass", "memberOf", "name", "cn", "distinguishedName",
        "sAMAccountName", "userPrincipalName", "displayName", "givenName", "sn", "description",
        "telephoneNumber", "mail", "streetAddress", "postalCode", "l", "st", "co", "c","userAccountControl"
    };

    internal LdapProvider(LdapOptions options)
    {
        _ldapOptions = options;
    }

    public LdapProvider(IOptionsSnapshot<LdapOptions> options)
    {
        _ldapOptions = options.Value;
    }

    private async Task<ILdapConnection> GetConnectionAsync()
    {
        if (_ldapConnection is { Connected: true })
        {
            return _ldapConnection;
        }
        _ldapConnection = new LdapConnection() { SecureSocketLayer = _ldapOptions.ServerPortSsl != 0 };
        //Connect function will create a socket connection to the server - Port 389 for insecure and 3269 for secure
        await _ldapConnection.ConnectAsync(_ldapOptions.ServerAddress,
            _ldapOptions.ServerPortSsl != 0 ? _ldapOptions.ServerPortSsl : _ldapOptions.ServerPort);
        //Bind function with null user dn and password value will perform anonymous bind to LDAP server
        await _ldapConnection.BindAsync(_ldapOptions.RootUserDn, _ldapOptions.RootUserPassword);

        return _ldapConnection;
    }

    public async Task<bool> AuthenticateAsync(string distinguishedName, string password)
    {
        using var ldapConnection = new LdapConnection() { SecureSocketLayer = _ldapOptions.ServerPortSsl != 0 };
        await ldapConnection.ConnectAsync(_ldapOptions.ServerAddress,
            ldapConnection.SecureSocketLayer ? _ldapOptions.ServerPortSsl : _ldapOptions.ServerPort);
        try
        {
            await ldapConnection.BindAsync(distinguishedName, password);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task DeleteUserAsync(string distinguishedName)
    {
        using (var ldapConnection = await GetConnectionAsync())
        {
            await ldapConnection.DeleteAsync(distinguishedName);
        }
    }

    public async Task AddUserAsync(LdapUser user, string password)
    {
        var dn = $"CN={user.FirstName} {user.LastName},{_ldapOptions.UserSearchBaseDn}";

        var attributeSet = new LdapAttributeSet
        {
            new LdapAttribute("instanceType", "4"),
            new LdapAttribute("objectCategory", $"CN=Users,{_ldapOptions.UserSearchBaseDn}"),
            new LdapAttribute("objectClass", new[] { "top", "person", "organizationalPerson", "user" }),
            new LdapAttribute("name", user.Name),
            new LdapAttribute("cn", $"{user.FirstName} {user.LastName}"),
            new LdapAttribute("sAMAccountName", user.SamAccountName),
            new LdapAttribute("userPrincipalName", user.UserPrincipalName),
            new LdapAttribute("unicodePwd", Convert.ToBase64String(Encoding.Unicode.GetBytes($"\"{password}\""))),
            new LdapAttribute("userAccountControl", "512"),
            new LdapAttribute("givenName", user.FirstName),
            new LdapAttribute("sn", user.LastName),
            new LdapAttribute("mail", user.EmailAddress),
            new LdapAttribute("company", user.Company),
            new LdapAttribute("department", user.Department),
            new LdapAttribute("title", user.Title)
        };

        attributeSet.AddAttribute("displayName", user.DisplayName);
        attributeSet.AddAttribute("description", user.Description);
        attributeSet.AddAttribute("telephoneNumber", user.Phone);
        attributeSet.AddAttribute("streetAddress", user.Address.Street);
        attributeSet.AddAttribute("l", user.Address.City);
        attributeSet.AddAttribute("postalCode", user.Address.PostalCode);
        attributeSet.AddAttribute("st", user.Address.StateName);
        attributeSet.AddAttribute("co", user.Address.CountryName);
        attributeSet.AddAttribute("c", user.Address.CountryCode);

        var newEntry = new LdapEntry(dn, attributeSet);

        using var ldapConnection = await GetConnectionAsync();
        await ldapConnection.AddAsync(newEntry);
    }

    public async IAsyncEnumerable<LdapUser> GetAllUserAsync()
    {
        var filter = $"(&(objectCategory=person)(objectClass=user))";
        var users = GetFilterLdapEntryAsync(_ldapOptions.UserSearchBaseDn, filter);
        await foreach (var user in users)
        {
            yield return CreateUser(user.Dn, user.GetAttributeSet());
        }
    }

    public async Task<List<LdapEntry>> GetPagingUserAsync(int pageSize)
    {
        using var ldapConnection = await GetConnectionAsync();
        return await ldapConnection.SearchUsingSimplePagingAsync(new SearchOptions(
                _ldapOptions.UserSearchBaseDn,
                LdapConnection.ScopeSub,
                "(&(objectCategory=person)(objectClass=user))",
                _attributes),
            pageSize);
    }

    public async Task<LdapUser?> GetUserByUserNameAsync(string userName)
    {
        var filter = $"(&(objectClass=user)(sAMAccountName={userName}))";
        var user = await FirstOrDefaultAsync(GetFilterLdapEntryAsync(_ldapOptions.UserSearchBaseDn, filter));
        return user == null ? null : CreateUser(user.Dn, user.GetAttributeSet());
    }

    public async Task<LdapUser?> GetUsersByEmailAddressAsync(string emailAddress)
    {
        var filter = $"(&(objectClass=user)(mail={emailAddress}))";
        var user = await FirstOrDefaultAsync(GetFilterLdapEntryAsync(_ldapOptions.UserSearchBaseDn, filter));
        return user == null ? null : CreateUser(user.Dn, user.GetAttributeSet());
    }

    private async IAsyncEnumerable<LdapEntry> GetFilterLdapEntryAsync(string baseDn, string filter)
    {
        using var ldapConnection = await GetConnectionAsync();
        var searchResults = await ldapConnection.SearchAsync(
            baseDn,
            LdapConnection.ScopeSub,
            filter,
            _attributes,
            false);
        await foreach (var searchResult in searchResults)
        {
            yield return searchResult;
        }
    }

    private static async Task<LdapEntry?> FirstOrDefaultAsync(IAsyncEnumerable<LdapEntry> enumerable)
    {
#if NET10_0
        return await enumerable.FirstOrDefaultAsync();
#else
        var users = enumerable.GetAsyncEnumerator();
        if (await users.MoveNextAsync())
            return users.Current;
        return null;
#endif

    }

    public async IAsyncEnumerable<LdapUser> GetUsersInGroupAsync(string groupName)
    {
        var group = await GetGroupAsync(groupName);
        if (group == null)
        {
            yield break;
        }
        var filter = $"(&(objectCategory=person)(objectClass=user)(memberOf={group.Dn}))";
        var users = GetFilterLdapEntryAsync(_ldapOptions.UserSearchBaseDn, filter);

        await foreach (var user in users)
        {
            yield return CreateUser(user.Dn, user.GetAttributeSet());
        }
    }

    public async Task<LdapEntry?> GetGroupAsync(string groupName)
    {
        var filter = $"(&(objectCategory=group)(objectClass=group)(cn={groupName}))";
        return await FirstOrDefaultAsync(GetFilterLdapEntryAsync(_ldapOptions.UserSearchBaseDn, filter));
    }

    public void Dispose()
    {
        Dispose(true);

        if (_ldapConnection != null)
        {
            _ldapConnection.Dispose();

            if (_ldapConnection.Connected) _ldapConnection.Disconnect();
        }

        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // Cleanup
    }

    private LdapUser CreateUser(string distinguishedName, LdapAttributeSet attributeSet)
    {
        var ldapUser = new LdapUser();

        ldapUser.ObjectSid = ObjectSidToString(attributeSet.GetAttribute("objectSid").ByteValue);
        ldapUser.ObjectGuid = ObjectGuidToString(attributeSet.GetAttribute("objectGUID").ByteValue);
        ldapUser.ObjectCategory = attributeSet.GetString("objectCategory");
        ldapUser.ObjectClass = attributeSet.GetString("objectClass");
        ldapUser.MemberOf = attributeSet.GetStringArray("memberOf");
        ldapUser.CommonName = attributeSet.GetString("cn");
        ldapUser.SamAccountName = attributeSet.GetString("sAMAccountName");
        ldapUser.UserPrincipalName = attributeSet.GetString("userPrincipalName");
        ldapUser.Name = attributeSet.GetString("name");
        ldapUser.DistinguishedName = attributeSet.GetString("distinguishedName");
        ldapUser.DisplayName = attributeSet.GetString("displayName");
        ldapUser.FirstName = attributeSet.GetString("givenName");
        ldapUser.LastName = attributeSet.GetString("sn");
        ldapUser.Description = attributeSet.GetString("description");
        ldapUser.Phone = attributeSet.GetString("telephoneNumber");
        ldapUser.EmailAddress = attributeSet.GetString("mail");
        ldapUser.Company = attributeSet.GetString("company");
        ldapUser.Department = attributeSet.GetString("department");
        ldapUser.Title = attributeSet.GetString("title");

        if (attributeSet.TryGetValue("userAccountControl", out var userAccountControlAttribute))
        {
            if (int.TryParse(userAccountControlAttribute.StringValue, out var userAccountControlValue))
            {
                ldapUser.UserAccountControl = (UserAccountControl)userAccountControlValue;
            }
        }

        ldapUser.Address = new LdapAddress
        {
            Street = attributeSet.GetString("streetAddress"),
            City = attributeSet.GetString("l"),
            PostalCode = attributeSet.GetString("postalCode"),
            StateName = attributeSet.GetString("st"),
            CountryName = attributeSet.GetString("co"),
            CountryCode = attributeSet.GetString("c")
        };
        attributeSet.TryGetValue("sAMAccountType", out var sAmAccountType);
        ldapUser.SamAccountType = int.Parse(sAmAccountType?.StringValue ?? "0");

        ldapUser.IsDomainAdmin = ldapUser.MemberOf.Contains("CN=Domain Admins," + _ldapOptions.BaseDn);

        return ldapUser;
    }

    private string ObjectGuidToString(byte[] bytes)
    {
        var strGUID = "";
        strGUID += AddLeadingZero(bytes[3] & 0xFF);
        strGUID += AddLeadingZero(bytes[2] & 0xFF);
        strGUID += AddLeadingZero(bytes[1] & 0xFF);
        strGUID += AddLeadingZero(bytes[0] & 0xFF);
        strGUID += "-";
        strGUID += AddLeadingZero(bytes[5] & 0xFF);
        strGUID += AddLeadingZero(bytes[4] & 0xFF);
        strGUID += "-";
        strGUID += AddLeadingZero(bytes[7] & 0xFF);
        strGUID += AddLeadingZero(bytes[6] & 0xFF);
        strGUID += "-";
        strGUID += AddLeadingZero(bytes[8] & 0xFF);
        strGUID += AddLeadingZero(bytes[9] & 0xFF);
        strGUID += "-";
        strGUID += AddLeadingZero(bytes[10] & 0xFF);
        strGUID += AddLeadingZero(bytes[11] & 0xFF);
        strGUID += AddLeadingZero(bytes[12] & 0xFF);
        strGUID += AddLeadingZero(bytes[13] & 0xFF);
        strGUID += AddLeadingZero(bytes[14] & 0xFF);
        strGUID += AddLeadingZero(bytes[15] & 0xFF);

        return strGUID;
    }

    private string ObjectSidToString(byte[] bytes)
    {
        StringBuilder strSID = new StringBuilder("S-");
        strSID.Append(bytes[0]).Append('-');
        // bytes[2..7] :
        StringBuilder tmpBuff = new StringBuilder();
        for (int t = 2; t <= 7; t++)
        {
            //var hexString = (bytes[t] & 0xFF).ToString("X");
            //tmpBuff.Append(hexString);
            tmpBuff.Append(AddLeadingZero((int)bytes[t] & 0xFF));
        }
        strSID.Append(Convert.ToInt64(tmpBuff.ToString(), 16));
        // bytes[1] : the sub authorities count
        int count = bytes[1];
        for (int i = 0; i < count; i++)
        {
            int currSubAuthOffset = i * 4;
            tmpBuff.Length = 0;
            tmpBuff.Append(string.Format("{0:X2}{1:X2}{2:X2}{3:X2}",
                (bytes[11 + currSubAuthOffset] & 0xFF),
                (bytes[10 + currSubAuthOffset] & 0xFF),
                (bytes[9 + currSubAuthOffset] & 0xFF),
                (bytes[8 + currSubAuthOffset] & 0xFF)));

            strSID.Append('-').Append(Convert.ToInt64(tmpBuff.ToString(), 16));
        }
        return strSID.ToString();
    }

    string AddLeadingZero(int k)
    {
        return (k <= 0xF) ? "0" + Int2String(k) : Int2String(k);
    }

    string Int2String(int kb)
    {
        byte[] bytes = new byte[1];
        bytes[0] = (byte)(kb & 0xFF);
        return Convert.ToHexString(bytes);
    }
}
