// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration;

[Flags]
public enum SectionTypes
{
    Local = 0b_0000_0001, //1
    ConfigurationApi = 0b_0000_0010, //2
    All = Local | ConfigurationApi
}
