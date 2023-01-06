// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Data;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Ddd.Domain.Entities;
global using Masa.BuildingBlocks.Ddd.Domain.Entities.Auditing;
global using Masa.BuildingBlocks.Ddd.Domain.Events;
global using Masa.BuildingBlocks.Ddd.Domain.Repositories;
global using Masa.BuildingBlocks.Ddd.Domain.Values;
global using Masa.Contrib.Ddd.Domain.Repository.EFCore.Tests.Domain.Entities;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Moq;
global using System;
global using System.Collections.Generic;
global using EntityState = Masa.BuildingBlocks.Data.UoW.EntityState;
