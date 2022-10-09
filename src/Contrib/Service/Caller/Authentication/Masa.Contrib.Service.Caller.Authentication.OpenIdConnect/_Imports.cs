// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Service.Caller;
global using System.Net.Http.Headers;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Cryptography;
global using IdentityModel;
global using IdentityModel.Client;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
global using Masa.Contrib.Service.Caller.Authentication.OpenIdConnect;
global using Microsoft.Extensions.DependencyInjection;
global using Masa.Contrib.Service.Caller.Authentication.OpenIdConnect.Jwt;
global using Microsoft.Extensions.DependencyInjection.Extensions;
