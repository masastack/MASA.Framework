// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Data;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.Isolation;
global using Masa.BuildingBlocks.Isolation.Environment;
global using Masa.BuildingBlocks.Isolation.MultiTenant;
global using Masa.Contrib.Data.EFCore;
global using Masa.Contrib.Data.EFCore.Filters;
global using Masa.Contrib.Data.UoW.EFCore;
global using Masa.Contrib.Isolation.UoW.EFCore.Internal;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.ChangeTracking;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using System.Linq.Expressions;
global using System.Reflection;
