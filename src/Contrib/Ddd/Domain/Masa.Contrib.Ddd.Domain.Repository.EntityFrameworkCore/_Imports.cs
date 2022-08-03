// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Ddd.Domain.Entities;
global using Masa.BuildingBlocks.Ddd.Domain.Repositories;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.Contrib.Ddd.Domain.Repository.EntityFrameworkCore.Internal;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using System.Linq.Expressions;
global using System.Reflection;
global using EntityState = Masa.BuildingBlocks.Data.UoW.EntityState;
