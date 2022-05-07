// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster.Internal.Options;

internal class MapTypeOptions
{
    public Type SourceType { get; } = default!;

    public Type DestinationType { get; } = default!;

    public ConstructorInfo Constructor { get; set; } = default!;

    public MapTypeOptions(Type sourceType, Type destinationType)
    {
        SourceType = sourceType;
        DestinationType = destinationType;
    }
}
