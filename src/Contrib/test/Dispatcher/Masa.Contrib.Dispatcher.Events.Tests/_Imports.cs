// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.Dispatcher.IntegrationEvents;
global using Masa.Contrib.Data.EntityFrameworkCore;
global using Masa.Contrib.Data.UoW.EF;
global using Masa.Contrib.Dispatcher.Events.CheckMethodsParameter.Tests.Events;
global using Masa.Contrib.Dispatcher.Events.CheckMethodsParameterNotNull.Tests.Events;
global using Masa.Contrib.Dispatcher.Events.CheckMethodsParameterType.Tests.Events;
global using Masa.Contrib.Dispatcher.Events.CheckMethodsType.Tests.Events;
global using Masa.Contrib.Dispatcher.Events.Enums;
global using Masa.Contrib.Dispatcher.Events.HandlerOrder.Tests.Events;
global using Masa.Contrib.Dispatcher.Events.OrderEqualBySaga.Tests.Events;
global using Masa.Contrib.Dispatcher.Events.Tests.Events;
global using Masa.Contrib.Dispatcher.Events.Tests.Middleware;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Moq;
global using System.Reflection;
