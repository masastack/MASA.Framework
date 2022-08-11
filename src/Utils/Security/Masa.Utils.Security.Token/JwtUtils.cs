// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Token;

public class JwtUtils
{
    internal static IServiceCollection Services { get; set; }

    private static IServiceProvider? _serviceProvider;

    private static IServiceProvider GetServiceProvider()
        => _serviceProvider ??= Services.BuildServiceProvider().CreateScope().ServiceProvider;

    private static IJwtProvider GetJwtProvider() => GetServiceProvider().GetRequiredService<IJwtProvider>();

    public static string CreateToken(string value, TimeSpan timeout)
        => GetJwtProvider().CreateToken(value, timeout);

    public static string CreateToken(Claim[] claims, TimeSpan timeout)
        => GetJwtProvider().CreateToken(claims, timeout);

    public static bool IsValid(string token, string value)
        => GetJwtProvider().IsValid(token, value);

    public static bool IsValid(string token, out SecurityToken? securityToken, out ClaimsPrincipal? claimsPrincipal)
        => GetJwtProvider().IsValid(token, out securityToken, out claimsPrincipal);
}
