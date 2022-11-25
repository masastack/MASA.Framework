// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Data;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Ddd.Domain.Entities;
global using Masa.BuildingBlocks.Ddd.Domain.Entities.Auditing;
global using Masa.BuildingBlocks.Ddd.Domain.Values;
global using Masa.Contrib.Ddd.Domain.Repository.EFCore.IntegrationTests.Domain.Entities;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Moq;
global using System;
global using System.Collections.Generic;
global using EntityState = Masa.BuildingBlocks.Data.UoW.EntityState;
global using System.Reflection;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Masa.Contrib.Ddd.Domain.Repository.EFCore.IntegrationTests.EntityTypeConfigurations;
global using Masa.BuildingBlocks.Ddd.Domain.Repositories;
global using Masa.Contrib.Data.UoW.EFCore;
