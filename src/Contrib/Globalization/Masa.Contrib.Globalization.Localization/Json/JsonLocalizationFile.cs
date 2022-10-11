// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.Localization;

public class JsonLocalizationFile
{
    /// <summary>
    /// Culture name
    /// eg: en,en-us, zh-CN
    /// </summary>
    public string Culture { get; set; }

    public Dictionary<string, string> Texts { get; set; }

    public JsonLocalizationFile()
    {
        Texts = new Dictionary<string, string>();
    }
}
