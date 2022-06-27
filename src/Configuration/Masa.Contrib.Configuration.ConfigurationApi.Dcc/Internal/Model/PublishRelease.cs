// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal.Model;

internal class PublishRelease
{
    private ConfigFormats _configFormat;
    public ConfigFormats ConfigFormat
    {
        get
        {
            try
            {
                if (_configFormat == 0 && !string.IsNullOrWhiteSpace(FormatLabelCode))
                    _configFormat = (ConfigFormats)Enum.Parse(typeof(ConfigFormats), FormatLabelCode);
            }
            catch (Exception ex)
            {
                throw new NotSupportedException("Unsupported configuration type", ex);
            }

            return _configFormat;
        }
        set
        {
            _configFormat = value;
        }
    }

    public string? FormatLabelCode { get; set; }

    public string? Content { get; set; }
}
