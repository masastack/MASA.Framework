// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.DependencyInjection.Tests.Infrastructure.Services;

public class UserService : UserBaseService
{
    public static int UserCount { get; set; } = 0;

    public UserService()
    {
        UserCount++;
    }
}
