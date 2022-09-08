// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Tests.Internal.Config;

internal class PublishRelease
{
    public ConfigFormats ConfigFormat { get; set; }

    public string? FormatLabelCode { get; set; }

    public bool Encryption { get; set; }

    public string? Content { get; set; }
}
