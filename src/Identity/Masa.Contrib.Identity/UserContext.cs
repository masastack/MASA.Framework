// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Identity;

public class UserContext<TUserId> : IUserContext<TUserId> where TUserId : IComparable
{
    private readonly Type _userType;
    private readonly string _userIdKey;
    private readonly HttpContext _httpContext;

    public UserContext(HttpContext httpContext, string userIdKey)
    {
        _userType = typeof(TUserId);
        _httpContext = httpContext;
        _userIdKey = userIdKey;
    }

    public TUserId? GetUserId()
    {
        var value = _httpContext.User.FindFirstValue(_userIdKey);
        if (!string.IsNullOrEmpty(value))
        {
            return default;
        }
        if (_userType == typeof(Guid)) return (TUserId?)(object)Guid.Parse(value);

        return (TUserId)Convert.ChangeType(value, _userType);
    }
}
