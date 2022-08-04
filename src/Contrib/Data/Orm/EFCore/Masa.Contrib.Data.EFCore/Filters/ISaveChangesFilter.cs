// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Filters;

public interface ISaveChangesFilter
{
    void OnExecuting(ChangeTracker changeTracker);
}
