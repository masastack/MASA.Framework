// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Models;

public abstract class ResourceModel
{
    public bool Enabled { get; set; } = true;

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string? Description { get; set; }

    public bool ShowInDiscoveryDocument { get; set; } = true;

    [DisallowNull]
    public virtual ICollection<string> UserClaims { get; set; } = new HashSet<string>();

    public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
}

