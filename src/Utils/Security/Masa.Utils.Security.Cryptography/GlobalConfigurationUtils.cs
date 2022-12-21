// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Cryptography;

public static class GlobalConfigurationUtils
{
    #region Aes

    private static string _defaultAesEncryptKey = "masastack.com                   ";

    public static string DefaultAesEncryptKey
    {
        get => _defaultAesEncryptKey;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{nameof(DefaultAesEncryptKey)} cannot be empty", nameof(DefaultAesEncryptKey));

            _defaultAesEncryptKey = value;
        }
    }

    private static string _defaultAesEncryptIv = "AreyoumySnowman?";

    public static string DefaultAesEncryptIv
    {
        get => _defaultAesEncryptIv;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{nameof(DefaultAesEncryptIv)} cannot be empty", nameof(DefaultAesEncryptIv));

            _defaultAesEncryptIv = value;
        }
    }

    private static int _defaultAesEncryptKeyLength = 32;

    public static int DefaultAesEncryptKeyLength
    {
        get => _defaultAesEncryptKeyLength;
        set
        {
            if (value != 16 && value != 24 && value != 32)
                throw new ArgumentException("Aes key length can only be 16, 24 or 32 bits", nameof(DefaultAesEncryptKeyLength));

            _defaultAesEncryptKeyLength = value;
        }
    }

    #endregion

    #region Des

    private static string _defaultDesEncryptKey = "c7fac67c";

    public static string DefaultDesEncryptKey
    {
        get => _defaultDesEncryptKey;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{nameof(DefaultDesEncryptKey)} cannot be empty", nameof(DefaultDesEncryptKey));

            _defaultDesEncryptKey = value;
        }
    }

    private static string _defaultDesEncryptIv = "c7fac67c";

    public static string DefaultDesEncryptIv
    {
        get => _defaultDesEncryptIv;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{nameof(DefaultDesEncryptIv)} cannot be empty", nameof(DefaultDesEncryptIv));

            _defaultDesEncryptIv = value;
        }
    }

    #endregion
}
