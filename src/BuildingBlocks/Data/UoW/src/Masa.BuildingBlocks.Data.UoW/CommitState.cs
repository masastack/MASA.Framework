// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.UoW;

public enum CommitState
{
    /// <summary>
    /// A transaction is opened and the data has changed
    /// </summary>
    UnCommited,
    /// <summary>
    /// The transaction is not opened or the data has not changed
    /// </summary>
    Commited
}
