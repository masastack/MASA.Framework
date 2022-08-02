// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Reflection;
global using Microsoft.EntityFrameworkCore;
global using Moq;
global using Masa.BuildingBlocks.Ddd.Domain.Entities;
global using Masa.BuildingBlocks.Ddd.Domain.Entities.Auditing;
global using Masa.BuildingBlocks.Ddd.Domain.Values;
global using Masa.Contrib.Ddd.Domain.Repository.EF.Tests.Domain.Entities;
global using Masa.BuildingBlocks.Data.UoW;
global using EntityState = Masa.BuildingBlocks.Data.UoW.EntityState;
