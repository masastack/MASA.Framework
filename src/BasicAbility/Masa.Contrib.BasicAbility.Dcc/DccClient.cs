// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Dcc;

public class DccClient : IDccClient
{
    public DccClient(IDistributedCacheClient distributedCacheClient)
    {
        LabelService = new LabelService(distributedCacheClient);
    }

    public ILabelService LabelService { get; }
}
