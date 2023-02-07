// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Middleware;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class DisabledRouteAttribute : Attribute
{
}
