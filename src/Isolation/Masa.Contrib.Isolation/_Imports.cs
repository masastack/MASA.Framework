// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Configuration;
global using Masa.BuildingBlocks.Data;
global using Masa.BuildingBlocks.Data.Options;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.Isolation;
global using Masa.BuildingBlocks.Isolation.Environment;
global using Masa.BuildingBlocks.Isolation.Middleware;
global using Masa.BuildingBlocks.Isolation.MultiTenant;
global using Masa.BuildingBlocks.Isolation.Options;
global using Masa.Contrib.Isolation.Middleware;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using System.Linq.Expressions;
