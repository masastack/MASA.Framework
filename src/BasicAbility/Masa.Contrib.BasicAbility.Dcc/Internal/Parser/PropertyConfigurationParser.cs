// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Dcc.Internal.Parser;

internal class PropertyConfigurationParser
{
    public static IDictionary<string, string>? Parse(string raw, JsonSerializerOptions serializerOption)
        => JsonSerializer.Deserialize<List<Property>>(raw, serializerOption)?.ToDictionary(k => k.Key, v => v.Value);
}
