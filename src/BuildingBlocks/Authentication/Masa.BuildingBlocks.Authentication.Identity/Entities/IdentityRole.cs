// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Identity;

public class IdentityRole<T>
{
    public T Id { get; set; }

    public string Name { get; set; }
}
