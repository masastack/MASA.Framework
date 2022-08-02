// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Isolation;

public interface IIsolation<TKey> : IMultiTenant<TKey>, IMultiEnvironment
    where TKey : IComparable
{
}

public interface IIsolation : IIsolation<Guid>
{
}
