// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.DynamicsCrm.Core.Configurations;

public class CrmConfiguration : ICrmConfiguration
{
    public Guid SystemUserId { get; set; }

    public Guid BusinessUnitId { get; set; }
}
