// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Contracts;

public class BaseMessage
{
    /// <summary>
    /// Unique Identifier
    /// </summary>
    protected Guid _correlationId = Guid.NewGuid();

    public Guid CorrelationId() => _correlationId;
}
