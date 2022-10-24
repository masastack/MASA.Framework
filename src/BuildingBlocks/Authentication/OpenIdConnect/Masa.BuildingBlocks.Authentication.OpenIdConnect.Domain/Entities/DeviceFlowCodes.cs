// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;

public class DeviceFlowCodes : FullAggregateRoot<Guid, Guid>
{
    /// <summary>
    /// Gets or sets the device code.
    /// </summary>
    /// <value>
    /// The device code.
    /// </value>
    public string DeviceCode { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user code.
    /// </summary>
    /// <value>
    /// The user code.
    /// </value>
    public string UserCode { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subject identifier.
    /// </summary>
    /// <value>
    /// The subject identifier.
    /// </value>
    public string SubjectId { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the session identifier.
    /// </summary>
    /// <value>
    /// The session identifier.
    /// </value>
    public string SessionId { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    /// <value>
    /// The client identifier.
    /// </value>
    public string ClientId { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the description the user assigned to the device being authorized.
    /// </summary>
    /// <value>
    /// The description.
    /// </value>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expiration.
    /// </summary>
    /// <value>
    /// The expiration.
    /// </value>
    public DateTime? Expiration { get; private set; }

    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    /// <value>
    /// The data.
    /// </value>
    public string Data { get; private set; } = string.Empty;
}
