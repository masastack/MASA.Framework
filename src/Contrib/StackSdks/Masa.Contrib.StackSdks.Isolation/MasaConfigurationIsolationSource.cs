// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Isolation;

internal class MasaConfigurationIsolationSource : IConfigurationSource
{
    readonly DccConfigurationIsolationRepository _configurationIsolationRepository;

    public MasaConfigurationIsolationSource(DccConfigurationIsolationRepository configurationIsolationRepository)
    {
        _configurationIsolationRepository = configurationIsolationRepository;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder) => new MasaConfigurationIsolationProvider(_configurationIsolationRepository);
}
