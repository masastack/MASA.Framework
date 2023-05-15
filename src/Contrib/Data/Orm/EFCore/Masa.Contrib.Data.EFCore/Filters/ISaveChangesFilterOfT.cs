// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public interface ISaveChangesFilter<TDbContext> : ISaveChangesFilter
    where TDbContext : DbContext, IMasaDbContext
{

}
