// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth.Infrastructure;

public class BaseSingleton
{
    static BaseSingleton()
    {
        AllSingletons = new Dictionary<Type, object>();
    }

    public static IDictionary<Type, object> AllSingletons { get; }
}
