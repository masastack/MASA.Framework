// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Isolation.MultiTenant;

public interface IMultiTenant : IMultiTenant<Guid>
{
}

public interface IMultiTenant<TKey> where TKey : IComparable
{
    /// <summary>
    /// The framework is responsible for the assignment operation, no manual assignment is required
    /// </summary>
    public TKey TenantId { get; set; }
}
