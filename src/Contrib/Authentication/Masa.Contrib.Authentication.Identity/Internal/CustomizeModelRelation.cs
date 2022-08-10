// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Identity.Internal;

internal class CustomizeModelRelation
{
    /// <summary>
    /// For creating custom user models
    /// </summary>
    public Func<object[], object> Func { get; set; }

    public Dictionary<PropertyInfo, MethodInfo> Setters { get; set; }

    public CustomizeModelRelation(Func<object[], object> func, Dictionary<PropertyInfo, MethodInfo> setter)
    {
        Func = func;
        Setters = setter;
    }
}
