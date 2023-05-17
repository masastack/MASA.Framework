// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;

public class PublicConfigOptions : DccSectionOptionsBase
{
    public string AppId => DEFAULT_PUBLIC_ID;

    public static implicit operator DccSectionOptions(PublicConfigOptions publicConfigOptions)
        => new(
            publicConfigOptions.AppId,
            publicConfigOptions.Environment,
            publicConfigOptions.Cluster,
            publicConfigOptions.ConfigObjects,
            publicConfigOptions.Secret);
}
