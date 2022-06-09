// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth.Infrastructure;

public class Singleton<T> : BaseSingleton
{
    static T instance;

    public static T Instance
    {
        get => instance;
        set
        {
            instance = value;
            AllSingletons[typeof(T)] = value;
        }
    }
}
