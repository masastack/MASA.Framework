// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

public static class JsonSerializerExtensions
{
    /// <summary>
    /// Enable support for the <see langword="dynamic"/> feature.
    /// Changes the default handling for types specified as <see cref="object"/> from deserializing as
    /// <see cref="JsonElement"/> to instead deserializing as the one of the
    /// <see cref="JsonDynamicType"/>-derived types including:
    /// <see cref="JsonDynamicObject"/>,
    /// <see cref="JsonDynamicArray"/>,
    /// <see cref="JsonDynamicString"/>,
    /// <see cref="JsonDynamicNumber"/> and
    /// <see cref="JsonDynamicBoolean"/>.
    /// </summary>
    /// <remarks>
    /// When deserializing <see cref="JsonTokenType.StartObject"/>, <see cref="JsonDynamicObject"/>
    /// is returned which implements <see cref="System.Collections.IDictionary{string, object}"/>.
    /// When deserializing <see cref="JsonTokenType.StartArray"/>, <see cref="System.Collections.IList{object}"/>
    /// is returned which implements <see cref="System.Collections.IList{object}"/>.
    /// When deserializing <see cref="string"/>, <see cref="JsonDynamicString"/>
    /// is returned and supports an implicit cast to <see cref="string"/>.
    /// An explicit cast or assignment to other types, such as <see cref="JsonTokenType.DateTime"/>,
    /// is supported provided there is a custom converter for that Type.
    /// When deserializing <see cref="JsonTokenType.Number"/>, <see cref="JsonDynamicNumber"/> is returned.
    /// An explicit cast or assignment is required to the appropriate number type, such as <see cref="decimal"/> or <see cref="long"/>.
    /// When deserializing <see cref="JsonTokenType.True"/> and <see cref="JsonTokenType.False"/>,
    /// <see cref="JsonDynamicBool"/> is returned and supports an implicit cast to <see cref="bool"/>.
    /// An explicit cast or assignment to other types is supported provided there is a custom converter for that type.
    /// When deserializing <see cref="JsonTokenType.Null"/>, <see langword="null"/> is returned.
    /// </remarks>
    public static JsonSerializerOptions EnableDynamicTypes(this JsonSerializerOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        options.Converters.Add(new DynamicObjectConverter());
        return options;
    }
}
