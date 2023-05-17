// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration;

public interface IMasaConfigurationChangeListener
{
    /// <summary>
    /// 更希望获取指定key的更改情，并返回更新后的值
    /// </summary>
    /// <param name="action"></param>
    void AddChangeListener(Action<MasaConfigurationChangeOptions> action);

    void OnChanged(MasaConfigurationChangeOptions changeOptions);
}
