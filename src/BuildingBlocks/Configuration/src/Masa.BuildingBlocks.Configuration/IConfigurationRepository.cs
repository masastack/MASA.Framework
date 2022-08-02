// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration;

public interface IConfigurationRepository
{
    SectionTypes SectionType { get; }

    Properties Load();

    void AddChangeListener(IRepositoryChangeListener listener);

    void RemoveChangeListener(IRepositoryChangeListener listener);
}
