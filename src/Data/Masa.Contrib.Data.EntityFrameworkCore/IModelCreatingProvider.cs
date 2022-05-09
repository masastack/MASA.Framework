// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore;

public interface IModelCreatingProvider
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="modelBuilder"></param>
    void Configure(ModelBuilder modelBuilder);
}
