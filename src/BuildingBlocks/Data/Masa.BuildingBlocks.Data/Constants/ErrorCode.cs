// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.Constants;

public static class ErrorCode
{
    private const string FRAMEWORK = "MF";

    #region Type

    private const string ARGUMENT = $"{FRAMEWORK}ARG";

    #endregion

    #region Argument

    /// <summary>
    ///
    /// </summary>
    public const string ARGUMENT_ERROR = $"{ARGUMENT}0001";

    /// <summary>
    /// Value cannot be null. (Parameter 'value')
    /// </summary>
    public const string ARGUMENT_NULL = $"{ARGUMENT}0002";

    /// <summary>
    /// Value cannot be null and empty . (Parameter 'value')
    /// </summary>
    public const string ARGUMENT_NULL_OR_EMPTY = $"{ARGUMENT}0003";

    /// <summary>
    /// Value cannot be null and WhiteSpace . (Parameter 'value')
    /// </summary>
    public const string ARGUMENT_NULL_OR_WHITE_SPACE = $"{ARGUMENT}0004";

    /// <summary>
    /// Value must be between {0}-{1} . (Parameter 'value')
    /// </summary>
    public const string ARGUMENT_OUT_OF_RANGE = $"{ARGUMENT}0005";

    /// <summary>
    /// Value must be greater than {0} . (Parameter 'value')
    /// </summary>
    public const string ARGUMENT_GREATER_THAN = $"{ARGUMENT}0006";

    /// <summary>
    /// Value must be greater than or equal {0} . (Parameter 'value')
    /// </summary>
    public const string ARGUMENT_GREATER_THAN_OR_EQUAL = $"{ARGUMENT}0007";

    /// <summary>
    /// Value must be less than {0} . (Parameter 'value')
    /// </summary>
    public const string ARGUMENT_LESS_THAN = $"{ARGUMENT}0008";

    /// <summary>
    /// Value must be less than or equal {0} . (Parameter 'value')
    /// </summary>
    public const string ARGUMENT_LESS_THAN_OR_EQUAL = $"{ARGUMENT}0009";

    /// <summary>
    /// Value does not support {0}
    /// </summary>
    public const string ARGUMENT_NOT_SUPPORTED_SINGLE = $"{ARGUMENT}0010";

    /// <summary>
    /// Value does not support {0} or {1}. (Parameter 'value')
    /// </summary>
    public const string ARGUMENT_NOT_SUPPORTED_MULTI = $"{ARGUMENT}0011";

    #endregion

}
