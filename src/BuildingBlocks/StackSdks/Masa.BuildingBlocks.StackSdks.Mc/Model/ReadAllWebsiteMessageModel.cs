// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Mc.Model;

public class ReadAllWebsiteMessageModel : GetWebsiteMessageModel
{
    public ReadAllWebsiteMessageModel(int page, int pageSize, Dictionary<string, bool>? sorting = null) : base(page, pageSize, sorting)
    {

    }
}
