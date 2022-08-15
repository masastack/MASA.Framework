// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Authentication.Identity;
global using Masa.BuildingBlocks.Data;
global using Masa.Contrib.Authentication.Identity;
global using Masa.Contrib.Authentication.Identity.Const;
global using Masa.Contrib.Authentication.Identity.Internal;
global using Masa.Utils.Caching.Memory;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Options;
global using System.Linq.Expressions;
global using System.Reflection;
global using System.Security.Claims;
global using IdentityUser = Masa.BuildingBlocks.Authentication.Identity.IdentityUser;
