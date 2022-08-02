// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.Core;

public interface IResponseMessage
{
    Task<TResponse?> ProcessResponseAsync<TResponse>(HttpResponseMessage response, CancellationToken cancellationToken = default);

    Task ProcessResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken = default);
}
