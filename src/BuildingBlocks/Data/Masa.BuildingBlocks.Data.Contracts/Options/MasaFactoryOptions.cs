// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.Contracts;

public class MasaFactoryOptions<TRelationOptions> where TRelationOptions : MasaRelationOptions
{
    public List<TRelationOptions> Options { get; set; }
}
