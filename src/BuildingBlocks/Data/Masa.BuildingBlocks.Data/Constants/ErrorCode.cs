// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.Constants;

public static class ErrorCode
{
    public const string FRAMEWORK_PREFIX = "MF";

    #region Type

    /// <summary>
    /// Parameter error
    /// </summary>
    private const string ARGUMENT = $"{FRAMEWORK_PREFIX}ARG";

    /// <summary>
    /// Internal service error
    /// </summary>
    private const string InternalServer = $"{FRAMEWORK_PREFIX}Ser";

    #endregion

    #region Argument

    /// <summary>
    /// Value Error. (Parameter 'value')
    /// </summary>
    [Description("Value Error. (Parameter '{0}')")]
    public const string ARGUMENT_ERROR = $"{ARGUMENT}0001";

    /// <summary>
    /// Value cannot be null. (Parameter 'value')
    /// </summary>
    [Description("Value cannot be null. (Parameter '{0}')")]
    public const string ARGUMENT_NULL = $"{ARGUMENT}0002";

    /// <summary>
    /// Value cannot be null and empty . (Parameter 'value')
    /// </summary>
    [Description("Value cannot be null and empty. (Parameter '{0}')")]
    public const string ARGUMENT_NULL_OR_EMPTY = $"{ARGUMENT}0003";

    /// <summary>
    /// Value cannot be null and WhiteSpace . (Parameter 'value')
    /// </summary>
    [Description("Value cannot be null and WhiteSpace . (Parameter '{0}')")]
    public const string ARGUMENT_NULL_OR_WHITE_SPACE = $"{ARGUMENT}0004";

    /// <summary>
    /// Value must be between {1}-{2} . (Parameter 'value')
    /// </summary>
    [Description("Value must be between {1}-{2} . (Parameter '{0}')")]
    public const string ARGUMENT_OUT_OF_RANGE = $"{ARGUMENT}0005";

    /// <summary>
    /// Value must be greater than {1} . (Parameter 'value')
    /// </summary>
    [Description("Value must be greater than {1} . (Parameter '{0}')")]
    public const string ARGUMENT_GREATER_THAN = $"{ARGUMENT}0006";

    /// <summary>
    /// Value must be greater than or equal {1} . (Parameter 'value')
    /// </summary>
    [Description("Value must be greater than or equal {1} . (Parameter '{0}')")]
    public const string ARGUMENT_GREATER_THAN_OR_EQUAL = $"{ARGUMENT}0007";

    /// <summary>
    /// Value must be less than {1} . (Parameter 'value')
    /// </summary>
    [Description("Value must be less than {1} . (Parameter '{0}')")]
    public const string ARGUMENT_LESS_THAN = $"{ARGUMENT}0008";

    /// <summary>
    /// Value must be less than or equal {1} . (Parameter 'value')
    /// </summary>
    [Description("Value must be less than or equal {1} . (Parameter '{0}')")]
    public const string ARGUMENT_LESS_THAN_OR_EQUAL = $"{ARGUMENT}0009";

    /// <summary>
    /// Value cannot contain {1}. (Parameter 'value')
    /// </summary>
    [Description("Value cannot contain {1}. (Parameter '{0}')")]
    public const string ARGUMENT_NOT_SUPPORTED_SINGLE = $"{ARGUMENT}0010";

    /// <summary>
    /// Value cannot contain {1} or {2}. (Parameter 'value')
    /// </summary>
    [Description("Value cannot contain {1} or {2}. (Parameter '{0}')")]
    public const string ARGUMENT_NOT_SUPPORTED_MULTI = $"{ARGUMENT}0011";

    /// <summary>
    /// Value cannot be null or empty collection. (Parameter 'value')
    /// </summary>
    [Description("Value cannot be null or empty collection. (Parameter '{0}')")]
    public const string ARGUMENT_NULL_OR_EMPTY_COLLECTION = $"{ARGUMENT}0012";

    #endregion

    #region Other

    /// <summary>
    /// Internal service error
    /// </summary>
    [Description("Internal service error")]
    public const string INTERNAL_SERVER_ERROR = $"{InternalServer}0001";

    #endregion

    private static Dictionary<string, string?> ErrorCodeMessageDictionary = new();

    static ErrorCode()
    {
        var classType = typeof(ErrorCode);
        var fields = classType.GetFields(BindingFlags.Static | BindingFlags.Public);
        foreach (var field in fields)
        {
            var errorMessage = AttributeUtils.GetDescriptionByField(field);

            ErrorCodeMessageDictionary.Add(field.GetRawConstantValue()!.ToString(), errorMessage);
        }
    }

    public static string? GetErrorMessage(string errorCode)
    {
        if (ErrorCodeMessageDictionary.TryGetValue(errorCode, out string? errorMessage))
            return errorMessage;

        return null;
    }
}
