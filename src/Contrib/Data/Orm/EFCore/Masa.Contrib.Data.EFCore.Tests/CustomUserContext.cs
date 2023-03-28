// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests;

public class CustomUserContext: IUserContext
{
    public bool IsAuthenticated { get; }

    private readonly string _userId;
    public string? UserId => _userId;

    public string? UserName { get; }

    public CustomUserContext(string? userId)
    {
        _userId = userId;
    }

    public TUserId? GetUserId<TUserId>()
    {
        throw new NotImplementedException();
    }

    public IdentityUser? GetUser()
    {
        throw new NotImplementedException();
    }

    public TIdentityUser? GetUser<TIdentityUser>() where TIdentityUser : IIdentityUser
    {
        throw new NotImplementedException();
    }

    public IEnumerable<TRoleId> GetUserRoles<TRoleId>()
    {
        throw new NotImplementedException();
    }
}
