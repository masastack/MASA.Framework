// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete;

public interface IAutoCompleteFactory : IMasaFactory<IManualAutoCompleteClient>
{
    [Obsolete("Use Create() instead")]
    IAutoCompleteClient CreateClient();

    [Obsolete("Use Create(name) instead")]
    IAutoCompleteClient CreateClient(string name);
}
