// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Authentication.Identity;
global using Masa.BuildingBlocks.Data.Contracts.DataFiltering;
global using Masa.BuildingBlocks.Ddd.Domain.Entities.Auditing;
global using Masa.Contrib.Data.Contracts.EntityFrameworkCore.DataFiltering;
global using Masa.Contrib.Data.Contracts.EntityFrameworkCore.Internal;
global using Masa.Contrib.Data.Contracts.EntityFrameworkCore.Options;
global using Masa.Contrib.Data.EntityFrameworkCore;
global using Masa.Contrib.Data.EntityFrameworkCore.Filters;
global using Masa.Utils.Caching.Memory;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.ChangeTracking;
global using Microsoft.EntityFrameworkCore.Metadata;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using System.Linq.Expressions;
global using System.Reflection;
