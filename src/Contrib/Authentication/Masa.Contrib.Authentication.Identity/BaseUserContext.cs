// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Identity;

public class BaseUserContext : IUserContext
{
    private readonly IUserContext _userContext;
    protected ITypeConvertProvider TypeConvertProvider { get; }

    public bool IsAuthenticated => _userContext.IsAuthenticated;
    public string? UserId => _userContext.UserId;
    public string? UserName => _userContext.UserName;

    public BaseUserContext(IServiceProvider serviceProvider)
    {
        _userContext = serviceProvider.GetRequiredService<IUserContext>();
        TypeConvertProvider = serviceProvider.GetRequiredService<ITypeConvertProvider>();
    }

    public TUserId? GetUserId<TUserId>() => _userContext.GetUserId<TUserId>();

    public IdentityUser? GetUser() => GetUser<IdentityUser>();

    public TIdentityUser? GetUser<TIdentityUser>() where TIdentityUser : IIdentityUser
        => _userContext.GetUser<TIdentityUser>();

    public IEnumerable<TRoleId> GetUserRoles<TRoleId>() => _userContext.GetUserRoles<TRoleId>();
}
