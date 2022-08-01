// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public enum SequentialGuidType
{
    /// <summary>
    /// The GUID should be sequential when formatted using the
    /// Used by MySql and PostgreSql.
    /// </summary>
    SequentialAsString,

    /// <summary>
    /// The GUID should be sequential when formatted using the
    /// Used by Oracle.
    /// </summary>
    SequentialAsBinary,

    /// <summary>
    /// The sequential portion of the GUID should be located at the end
    /// of the Data4 block.
    /// Used by SqlServer.
    /// </summary>
    SequentialAtEnd
}
