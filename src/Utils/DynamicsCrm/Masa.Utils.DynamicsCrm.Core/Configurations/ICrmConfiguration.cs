// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.DynamicsCrm.Core.Configurations;

public interface ICrmConfiguration
{
    Guid SystemUserId { get; set; }

    Guid BusinessUnitId { get; set; }
}
