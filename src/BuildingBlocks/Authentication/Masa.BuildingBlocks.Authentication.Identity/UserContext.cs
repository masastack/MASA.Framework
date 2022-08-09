// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Identity;

public abstract class UserContext : IUserSetter, IUserContext
{
    private readonly AsyncLocal<object?> _currentUser = new();

    public bool IsAuthenticated => GetUserSimple() != null;

    public string? UserId => GetUserSimple()?.Id;

    public string? UserName => GetUserSimple()?.UserName;

    protected ITypeConvertProvider TypeConvertProvider { get; }

    public UserContext(ITypeConvertProvider typeConvertProvider) => TypeConvertProvider = typeConvertProvider;

    protected abstract object? GetUser();

    protected abstract IdentityUser? GetUserBasicInfo();

    public TUserId? GetUserId<TUserId>()
    {
        var userId = UserId;
        if (userId == null)
            return default;

        return TypeConvertProvider.ConvertTo<TUserId>(userId);
    }

    public TIdentityUser? GetUser<TIdentityUser>() where TIdentityUser : IIdentityUser
    {
        var user = _currentUser.Value ?? GetUser();
        return user == null ? default : (TIdentityUser)user;
    }

    public IIdentityUser? GetUserSimple() => GetUser<IdentityUser>();

    public IDisposable Change<TIdentityUser>(TIdentityUser identityUser) where TIdentityUser : IIdentityUser
    {
        var user = GetUser();
        _currentUser.Value = identityUser;
        return new DisposeAction(() => _currentUser.Value = user);
    }

    public IEnumerable<TRoleId> GetUserRoles<TRoleId>()
    {
        return GetUserSimple()?.Roles.Select(r => TypeConvertProvider.ConvertTo<TRoleId>(r)) ?? new List<TRoleId>();
    }
}
