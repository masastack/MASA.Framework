// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration;

internal class LocalMasaConfigurationRepository : AbstractConfigurationRepository
{
    public override SectionTypes SectionType => SectionTypes.Local;

    private Properties _data = new();

    public LocalMasaConfigurationRepository(
        IConfiguration configuration,
        ILoggerFactory? loggerFactory)
        : base(loggerFactory)
    {
        Initialize(configuration);

        ChangeToken.OnChange(configuration.GetReloadToken, () =>
        {
            Initialize(configuration);
            FireRepositoryChange(SectionType, Load());
        });
    }

    private void Initialize(IConfiguration configuration)
    {
        var data = configuration.ConvertToDictionary();
        _data = new Properties(data);
    }

    public override Properties Load()
    {
        return _data;
    }
}
