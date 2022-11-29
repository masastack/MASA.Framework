// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticsearch.Constants;

internal static class MappingConstant
{
    public const string PROPERTY = "properties";
    public const string TYPE = "type";
    public const string FIELD = "fields";
    /// <summary>
    /// when field is keyword, the maximum field value length, fields beyond this length will not be indexed, but will be stored
    /// </summary>
    public const string MAXLENGTH = "ignore_above";
    public const string KEYWORD = "keyword";
}
