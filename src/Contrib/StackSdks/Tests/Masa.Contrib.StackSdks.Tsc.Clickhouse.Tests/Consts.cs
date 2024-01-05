// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Clickhouse.Tests;

internal class Consts
{
    public const string ConnectionString = "Compress=True;CheckCompressedHash=False;Compressor=lz4;SocketTimeout=5000;Host=localhost;Port=9000;Database=default;User=default";
    //public const string ConnectionString = "Compress=True;CheckCompressedHash=False;Compressor=lz4;SocketTimeout=5000;Host=192.168.51.234;Port=19003;Database=default;User=default";
}
