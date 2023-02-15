// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Authentication.OpenIdConnect.Cache.Caches;
global using Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;
global using Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Repositories;
global using Masa.Contrib.Authentication.OpenIdConnect.EFCore.Caches;
global using Masa.Contrib.Authentication.OpenIdConnect.EFCore.DbContexts;
global using Masa.Contrib.Authentication.OpenIdConnect.EFCore.Options;
global using Masa.Contrib.Caching.Distributed.StackExchangeRedis;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Moq;
