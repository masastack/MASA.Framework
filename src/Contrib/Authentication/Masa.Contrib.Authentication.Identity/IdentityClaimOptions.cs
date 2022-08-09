// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Identity;

public class IdentityClaimOptions
{
    private Dictionary<string, string> _mapping = new();
    private MemoryCache<string, string> Mappings { get; } = new();

    private string? _userId;

    public string? UserId
    {
        get => _userId ??= GetClaimType(nameof(UserId));
        set
        {
            if (value != null)
            {
                _userId = value;
                Mapping(nameof(UserId), value);
            }
        }
    }

    private string? _userName;

    public string? UserName
    {
        get => _userName ??= GetClaimType(nameof(UserName));
        set
        {
            if (value != null)
            {
                _userName = value;
                Mapping(nameof(UserName), value);
            }
        }
    }

    private string? _role;

    public string? Role
    {
        get => _role ??= GetClaimType(nameof(Role));
        set
        {
            if (value != null)
            {
                _role = value;
                Mapping(nameof(Role), value);
            }
        }
    }

    private string? _tenantId;

    public string? TenantId
    {
        get => _tenantId ??= GetClaimType(nameof(TenantId));
        set
        {
            if (value != null)
            {
                _tenantId = value;
                Mapping(nameof(TenantId), value);
            }
        }
    }

    private string? _environment;

    public string? Environment
    {
        get => _environment ??= GetClaimType(nameof(Environment));
        set
        {
            if (value != null)
            {
                _environment = value;
                Mapping(nameof(Environment), value);
            }
        }
    }

    internal bool IsInitialize { get; set; }

    public IdentityClaimOptions()
    {
        _mapping.Add(nameof(IIdentityUser.Id), nameof(UserId));
        _mapping.Add(nameof(IIdentityUser.Roles), nameof(Role));
    }

    public IdentityClaimOptions Mapping(string name, string claimType)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        Mappings.AddOrUpdate(name.ToLower(), (k) => claimType);
        return this;
    }

    public void Initialize()
    {
        if (!IsInitialize)
        {
            Mappings.TryAdd(nameof(UserId).ToLower(), (k) =>
            {
                _userId = ClaimType.DEFAULT_USER_ID;
                return _userId;
            });
            Mappings.TryAdd(nameof(UserName).ToLower(), (k) =>
            {
                _userName = ClaimType.DEFAULT_USER_NAME;
                return _userName;
            });
            Mappings.TryAdd(nameof(Role).ToLower(), (k) =>
            {
                _role = ClaimType.DEFAULT_USER_ROLE;
                return _role;
            });
            Mappings.TryAdd(nameof(TenantId).ToLower(), (k) =>
            {
                _tenantId = ClaimType.DEFAULT_TENANT_ID;
                return _tenantId;
            });
            Mappings.TryAdd(nameof(Environment).ToLower(), (k) =>
            {
                _environment = ClaimType.DEFAULT_ENVIRONMENT;
                return _environment;
            });
            IsInitialize = true;
        }
    }

    public string? GetClaimType(string name)
    {
        if (_mapping.ContainsKey(name))
            name = _mapping[name];

        Mappings.TryGet(name.ToLower(), out string? value);
        return value;
    }
}
