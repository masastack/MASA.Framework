// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.DependencyInjection.Tests.Services;

public class BaseService : ISingletonDependency, IAutoFireDependency
{
    public static int Count { get; set; } = 0;

    public BaseService()
    {
        Count++;
    }

    public BaseService(bool isChildren)
    {

    }
}
