// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Config
{
    public interface IMasaStackConfig
    {
        string GetValue(string key);

        void SetValue(string key, string value);
    }
}
