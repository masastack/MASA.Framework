// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

public static class ObjectExtensions
{
    public static string GetGenericTypeName(this object @object)
        => @object.GetType().GetGenericTypeName();
}
