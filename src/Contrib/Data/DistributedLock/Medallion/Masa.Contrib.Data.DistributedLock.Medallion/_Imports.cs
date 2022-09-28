// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Data;
global using Masa.Contrib.Data.DistributedLock.Medallion;
global using Masa.Contrib.Data.DistributedLock.Medallion.Internal;
global using Medallion.Threading;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using IMasaDistributedLock = Masa.BuildingBlocks.Data.IDistributedLock;
