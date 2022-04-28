// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Dcc.Tests.Internal.Common;

internal static class SerializeCommon
{
    public static string Serialize(this object obj, JsonSerializerOptions? jsonSerializerOptions)
        => JsonSerializer.Serialize(obj, jsonSerializerOptions);
}
