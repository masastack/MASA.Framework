// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Data;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.Isolation;
global using Masa.Contrib.Dispatcher.Events;
global using Masa.Contrib.Isolation.MultiEnvironment;
global using Masa.Contrib.Isolation.MultiTenant;
global using Masa.Contrib.Isolation.UoW.EFCore.Web.Tests.Events;
global using Masa.Utils.Security.Cryptography;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Data.Sqlite;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Threading.Tasks;
