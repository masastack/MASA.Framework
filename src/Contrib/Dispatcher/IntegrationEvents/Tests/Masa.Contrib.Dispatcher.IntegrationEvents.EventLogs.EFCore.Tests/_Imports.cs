// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.Dispatcher.IntegrationEvents;
global using Masa.BuildingBlocks.Dispatcher.IntegrationEvents.Logs;
global using Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore.Tests.Domain.Entities;
global using Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore.Tests.Events;
global using Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore.Tests.Infrastructure;
global using Microsoft.Data.Sqlite;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Storage;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Moq;
global using System.Data.Common;
global using System.Reflection;
global using System.Text.Json.Serialization;
