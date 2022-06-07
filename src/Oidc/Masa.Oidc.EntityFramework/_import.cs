// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Masa.BuildingBlocks.Oidc.Domain.Entities;
global using Microsoft.Extensions.DependencyInjection;
global using Masa.Oidc.EntityFramework.DbContexts;
global using Masa.BuildingBlocks.Oidc.Domain.Repositories;
global using Masa.Contrib.Ddd.Domain.Repository.EF;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Dispatcher.Events;
