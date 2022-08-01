// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster;

public interface IMappingConfigProvider
{
    TypeAdapterConfig GetConfig(Type sourceType, Type destinationType, MapOptions? options = null);
}
