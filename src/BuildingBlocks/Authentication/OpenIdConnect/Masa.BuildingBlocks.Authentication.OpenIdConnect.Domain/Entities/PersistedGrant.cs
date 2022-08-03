// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;

public class PersistedGrant : FullAggregateRoot<int, Guid>
{
    public string Key { get; private set; } = null!;

    public string Type { get; private set; } = string.Empty;

    public string SubjectId { get; private set; } = string.Empty;

    public string SessionId { get; private set; } = string.Empty;

    public string ClientId { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public DateTime? Expiration { get; private set; }

    public DateTime? ConsumedTime { get; private set; }

    public string Data { get; private set; } = string.Empty;
}
