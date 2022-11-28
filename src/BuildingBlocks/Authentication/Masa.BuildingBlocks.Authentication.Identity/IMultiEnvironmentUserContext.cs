// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Identity;

/// <summary>
/// When the user logs in, the environment information of the current user can be obtained
/// </summary>
public interface IMultiEnvironmentUserContext : IUserContext
{
    string? Environment { get; }
}
