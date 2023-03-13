// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masa.BuildingBlocks.Caching
{
    public interface IDistributedCacheClientDisposeWrapper : IDistributedCacheClient, IDisposable
    {
    }
}
