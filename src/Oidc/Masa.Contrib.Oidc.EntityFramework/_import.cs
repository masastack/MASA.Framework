// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Masa.BuildingBlocks.Oidc.Domain.Entities;
global using Microsoft.Extensions.DependencyInjection;
global using Masa.Contrib.Oidc.EntityFramework.DbContexts;
global using Masa.BuildingBlocks.Oidc.Domain.Repositories;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.Oidc.Cache.Caches;
global using Masa.BuildingBlocks.Ddd.Domain.Repositories;
global using System.Linq.Expressions;
global using Masa.Contrib.Oidc.EntityFramework.Repositories;
global using Masa.BuildingBlocks.Oidc.Models.Constans;
global using Masa.Contrib.Oidc.EntityFramework.Caches;
