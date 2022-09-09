// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Moq;
global using Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;
global using Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Repositories;
global using Masa.Contrib.Authentication.OpenIdConnect.EFCore.DbContexts;
global using Masa.Contrib.Authentication.OpenIdConnect.EFCore.Options;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Masa.Contrib.Authentication.OpenIdConnect.EFCore.Repositories;
global using Masa.Contrib.Isolation.UoW.EFCore;
global using Masa.Contrib.Ddd.Domain.Repository.EFCore;
global using Masa.Contrib.Data.UoW.EFCore;
global using Masa.Contrib.Dispatcher.Events;
global using Masa.Contrib.Dispatcher.IntegrationEvents;
global using Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore;
global using Microsoft.Extensions.DependencyInjection.Extensions;
