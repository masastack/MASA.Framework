// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using FluentValidation;
global using FluentValidation.AspNetCore;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Ddd.Domain.Entities;
global using Masa.BuildingBlocks.Ddd.Domain.Repositories;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.Dispatcher.IntegrationEvents;
global using Masa.BuildingBlocks.ReadWriteSpliting.Cqrs.Queries;
global using Masa.Contrib.Data.Contracts.EntityFrameworkCore;
global using Masa.Contrib.Data.EntityFrameworkCore;
global using Masa.Contrib.Data.UoW.EF;
global using Masa.Contrib.Ddd.Domain.Repository.EntityFrameworkCore;
global using Masa.Contrib.Dispatcher.Events;
global using Masa.Contrib.Dispatcher.IntegrationEvents;
global using Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Masa.EventBus.IntegrationTests.Domain.Aggregate;
global using Masa.EventBus.IntegrationTests.Infrastructure;
global using Masa.EventBus.IntegrationTests.Infrastructure.Extensions;
global using Masa.EventBus.IntegrationTests.Infrastructure.Middleware;
