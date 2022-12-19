// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Cryptography;

public static class GlobalConfigurationUtils
{
    private static string _defaultEncryptKey = "masastack.com                   ";

    public static string DefaultEncryptKey
    {
        get => _defaultEncryptKey;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{nameof(DefaultEncryptKey)} cannot be empty", nameof(DefaultEncryptKey));

            _defaultEncryptKey = value;
        }
    }
}
