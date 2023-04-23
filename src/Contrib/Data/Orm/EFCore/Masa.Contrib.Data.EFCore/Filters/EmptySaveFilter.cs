// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public class EmptySaveFilter<TDbContext> : ISaveChangesFilter<TDbContext>
    where TDbContext : DefaultMasaDbContext, IMasaDbContext
{
    public void OnExecuting(ChangeTracker changeTracker)
    {
        //Empty implementation, no processing required
    }
}
