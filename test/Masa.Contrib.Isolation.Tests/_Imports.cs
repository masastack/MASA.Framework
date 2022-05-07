// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.Isolation;
global using Masa.BuildingBlocks.Isolation.Environment;
global using Masa.BuildingBlocks.Isolation.MultiTenant;
global using Masa.BuildingBlocks.Isolation.Options;
global using Masa.BuildingBlocks.Isolation.Parser;
global using Masa.Contrib.Isolation.MultiEnvironment;
global using Masa.Contrib.Isolation.MultiTenant;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Primitives;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Moq;
global using System;
global using System.Linq;
global using Masa.BuildingBlocks.Data;
global using Masa.BuildingBlocks.Data.Options;
global using Masa.BuildingBlocks.Isolation.Middleware;
