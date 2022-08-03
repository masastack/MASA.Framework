// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Data;
global using Masa.BuildingBlocks.Data.Contracts.DataFiltering;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.Isolation.Environment;
global using Masa.BuildingBlocks.Isolation.MultiTenant;
global using Masa.BuildingBlocks.Isolation.Options;
global using Masa.Contrib.Data.Contracts.EntityFrameworkCore;
global using Masa.Contrib.Data.EntityFrameworkCore;
global using Masa.Contrib.Isolation.MultiEnvironment;
global using Masa.Contrib.Isolation.MultiTenant;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.Data.Sqlite;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Moq;
global using System;
global using System.IO;
global using System.Linq;
