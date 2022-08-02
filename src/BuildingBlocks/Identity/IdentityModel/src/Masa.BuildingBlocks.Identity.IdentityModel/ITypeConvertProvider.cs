// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Identity.IdentityModel;

public interface ITypeConvertProvider
{
    T ConvertTo<T>(string value);
}
