// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

public static class ByteExtensions
{
    public static string ToBase64String(this byte[] inArray)
        => Convert.ToBase64String(inArray);

    public static string ConvertToString(this byte[] inArray, Encoding encoding)
        => encoding.GetString(inArray);
}
