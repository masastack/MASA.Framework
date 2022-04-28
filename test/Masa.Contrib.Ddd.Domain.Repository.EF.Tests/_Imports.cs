// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Ddd.Domain.Entities;
global using Masa.BuildingBlocks.Ddd.Domain.Entities.Auditing;
global using Masa.BuildingBlocks.Ddd.Domain.Repositories;
global using Masa.BuildingBlocks.Ddd.Domain.Values;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.Contrib.Data.UoW.EF;
global using Masa.Contrib.Ddd.Domain.Repository.EF.CustomRepository.Tests.Repositories;
global using Masa.Contrib.Ddd.Domain.Repository.EF.Tests.Domain.Entities;
global using Masa.Contrib.Ddd.Domain.Repository.EF.Tests.Domain.Repositories;
global using Masa.Contrib.Ddd.Domain.Repository.EF.Tests.Infrastructure;
global using Masa.Utils.Data.EntityFrameworkCore;
global using Masa.Utils.Data.EntityFrameworkCore.Sqlite;
global using Microsoft.Data.Sqlite;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Storage;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Moq;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Reflection;
