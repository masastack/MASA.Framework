// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public interface IDataFilter
{
    IDisposable Enable<TFilter>() where TFilter : class;

    IDisposable Disable<TFilter>() where TFilter : class;

    bool IsEnabled<TFilter>() where TFilter : class;
}
