// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Identity;

public abstract class UserContext : IUserSetter, IUserContext
{
    private readonly AsyncLocal<Dictionary<Type, object?>?> _currentUser = new();

    public bool IsAuthenticated => GetUserSimple() != null;

    public string? UserId => GetUserSimple()?.Id;

    public string? UserName => GetUserSimple()?.UserName;

    protected ITypeConvertProvider TypeConvertProvider { get; }

    protected UserContext(ITypeConvertProvider typeConvertProvider)
    {
        TypeConvertProvider = typeConvertProvider;
        _currentUser.Value = new Dictionary<Type, object?>();
    }

    protected abstract object? GetUser(Type userType);

    public TUserId? GetUserId<TUserId>()
    {
        var userId = UserId;
        if (userId == null)
            return default;

        return TypeConvertProvider.ConvertTo<TUserId>(userId);
    }

    public IdentityUser? GetUser() => GetUser<IdentityUser>();

    public TIdentityUser? GetUser<TIdentityUser>() where TIdentityUser : IIdentityUser
    {
        var userModelType = typeof(TIdentityUser);
        if (!_currentUser.Value!.TryGetValue(userModelType, out var user) || user == null)
        {
            user ??= GetUser(userModelType);
            _currentUser.Value.TryAdd(userModelType, user);
        }
        return user == null ? default : (TIdentityUser)user;
    }

    public IIdentityUser? GetUserSimple() => GetUser<IdentityUser>();

    public IDisposable Change<TIdentityUser>(TIdentityUser identityUser) where TIdentityUser : IIdentityUser
    {
        var userModelType = typeof(TIdentityUser);
        var user = GetUser(userModelType);
        _currentUser.Value![userModelType] = identityUser;
        return new DisposeAction(() => _currentUser.Value[userModelType] = user);
    }

    public IEnumerable<TRoleId> GetUserRoles<TRoleId>()
    {
        return GetUserSimple()?.Roles.Select(r => TypeConvertProvider.ConvertTo<TRoleId>(r)) ?? new List<TRoleId>();
    }
}
