// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Cryptography;

public class GlobalConfigurationUtils
{
    private static string _defaultEncryKey = "masastack.com";

    public static string DefaultEncryKey
    {
        get => _defaultEncryKey;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{nameof(DefaultEncryKey)} cannot be empty", nameof(DefaultEncryKey));

            _defaultEncryKey = value;
        }
    }
}
