// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.Constants;

public static class ExceptionErrorCode
{
    public const string FRAMEWORK_PREFIX = "MF";

    #region Type

    /// <summary>
    /// Internal service error
    /// </summary>
    private const string INTERNAL_SERVER = $"{FRAMEWORK_PREFIX}SVR";

    /// <summary>
    /// parameter validation error
    /// </summary>
    private const string ARGUMENT = $"{FRAMEWORK_PREFIX}ARG";

    /// <summary>
    /// parameter validation error
    /// </summary>
    private const string BACKGROUND_JOB = $"{FRAMEWORK_PREFIX}BGJ";

    #endregion

    #region Argument Verify

    /// <summary>
    /// '{PropertyName}' is not a valid email address.
    /// </summary>
    [Description("'{0}' is not a valid email address.")]
    public const string EMAIL_VALIDATOR = $"{ARGUMENT}0001";

    /// <summary>
    /// '{PropertyName}' must be greater than or equal to '{ComparisonValue}'.
    /// </summary>
    [Description("'{0}' must be greater than or equal to '{1}'.")]
    public const string GREATER_THAN_OR_EQUAL_VALIDATOR = $"{ARGUMENT}0002";

    /// <summary>
    /// '{PropertyName}' must be greater than '{ComparisonValue}'.
    /// </summary>
    [Description("'{0}' must be greater than '{1}'.")]
    public const string GREATER_THAN_VALIDATOR = $"{ARGUMENT}0003";

    /// <summary>
    /// '{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.
    /// </summary>
    [Description("'{0}' must be between {1} and {2} characters. You entered {3} characters.")]
    public const string LENGTH_VALIDATOR = $"{ARGUMENT}0004";

    /// <summary>
    /// The length of '{PropertyName}' must be at least {MinLength} characters. You entered {TotalLength} characters.
    /// </summary>
    [Description("The length of '{0}' must be at least {1} characters. You entered {2} characters.")]
    public const string MINIMUM_LENGTH_VALIDATOR = $"{ARGUMENT}0005";

    /// <summary>
    /// The length of '{PropertyName}' must be {MaxLength} characters or fewer. You entered {TotalLength} characters.
    /// </summary>
    [Description("The length of '{0}' must be {1} characters or fewer. You entered {2} characters.")]
    public const string MAXIMUM_LENGTH_VALIDATOR = $"{ARGUMENT}0006";

    /// <summary>
    /// '{PropertyName}' must be less than or equal to '{ComparisonValue}'.
    /// </summary>
    [Description("'{0}' must be less than or equal to '{1}'.")]
    public const string LESS_THAN_OR_EQUAL_VALIDATOR = $"{ARGUMENT}0007";

    /// <summary>
    /// '{PropertyName}' must be less than '{ComparisonValue}'.
    /// </summary>
    [Description("'{0}' must be less than '{1}'.")]
    public const string LESS_THAN_VALIDATOR = $"{ARGUMENT}0008";

    /// <summary>
    /// '{PropertyName}' must not be empty.
    /// </summary>
    [Description("'{0}' must not be empty.")]
    public const string NOT_EMPTY_VALIDATOR = $"{ARGUMENT}0009";

    /// <summary>
    /// '{PropertyName}' must not be equal to '{ComparisonValue}'.
    /// </summary>
    [Description("'{0}' must not be equal to '{1}'.")]
    public const string NOT_EQUAL_VALIDATOR = $"{ARGUMENT}0010";

    /// <summary>
    /// '{PropertyName}' must not be empty.
    /// </summary>
    [Description("'{0}' must not be empty.")]
    public const string NOT_NULL_VALIDATOR = $"{ARGUMENT}0011";

    /// <summary>
    /// The specified condition was not met for '{0}'.
    /// </summary>
    [Description("The specified condition was not met for '{0}'.")]
    public const string PREDICATE_VALIDATOR = $"{ARGUMENT}0012";

    /// <summary>
    /// The specified condition was not met for '{0}'.
    /// </summary>
    [Description("The specified condition was not met for '{0}'.")]
    public const string ASYNC_PREDICATE_VALIDATOR = $"{ARGUMENT}0013";

    /// <summary>
    /// '{PropertyName}' is not in the correct format.
    /// </summary>
    [Description("'{0}' is not in the correct format.")]
    public const string REGULAR_EXPRESSION_VALIDATOR = $"{ARGUMENT}0014";

    /// <summary>
    /// '{PropertyName}' must be equal to '{ComparisonValue}'.
    /// </summary>
    [Description("'{0}' must be equal to '{1}'.")]
    public const string EQUAL_VALIDATOR = $"{ARGUMENT}0015";

    /// <summary>
    /// '{PropertyName}' must be {MaxLength} characters in length. You entered {TotalLength} characters.
    /// </summary>
    [Description("'{0}' must be {1} characters in length. You entered {2} characters.")]
    public const string EXACT_LENGTH_VALIDATOR = $"{ARGUMENT}0016";

    /// <summary>
    /// '{PropertyName}' must be between {From} and {To}. You entered {PropertyValue}.
    /// </summary>
    [Description("'{0}' must be between {1} and {2}. You entered {3}.")]
    public const string INCLUSIVE_BETWEEN_VALIDATOR = $"{ARGUMENT}0017";

    /// <summary>
    /// '{PropertyName}' must be between {From} and {To} (exclusive). You entered {PropertyValue}.
    /// </summary>
    [Description("'{0}' must be between {1} and {2} (exclusive). You entered {3}.")]
    public const string EXCLUSIVE_BETWEEN_VALIDATOR = $"{ARGUMENT}0018";

    /// <summary>
    /// '{PropertyName}' cannot be null and empty.
    /// </summary>
    [Description("'{0}' cannot be null and empty.")]
    public const string NOT_NULL_AND_EMPTY_VALIDATOR = $"{ARGUMENT}0019";

    /// <summary>
    /// '{PropertyName}' must not be more than {ExpectedPrecision} digits in total, with allowance for {ExpectedScale} decimals. {Digits} digits and {ActualScale} decimals were found.
    /// </summary>
    [Description(
        "'{0}' must not be more than {1} digits in total, with allowance for {2} decimals. {3} digits and {4} decimals were found.")]
    public const string SCALE_PRECISION_VALIDATOR = $"{ARGUMENT}0020";

    /// <summary>
    /// '{PropertyName}' must be empty.
    /// </summary>
    [Description("'{0}' must be empty.")] public const string EMPTY_VALIDATOR = $"{ARGUMENT}0021";

    /// <summary>
    /// '{PropertyName}' must be empty.
    /// </summary>
    [Description("'{0}' must be empty.")] public const string NULL_VALIDATOR = $"{ARGUMENT}0022";

    /// <summary>
    /// '{0}' has a range of values which does not include '{1}'.
    /// </summary>
    [Description("'{0}' has a range of values which does not include '{1}'.")]
    public const string ENUM_VALIDATOR = $"{ARGUMENT}0023";

    /// <summary>
    /// '{PropertyName}' must be between {MinLength} and {MaxLength} characters.
    /// </summary>
    [Description("'{0}' must be between {1} and {2} characters.")]
    public const string LENGTH_SIMPLE = $"{ARGUMENT}0024";

    /// <summary>
    /// The length of '{PropertyName}' must be at least {MinLength} characters.
    /// </summary>
    [Description("The length of '{0}' must be at least {1} characters.")]
    public const string MINIMUM_LENGTH_SIMPLE = $"{ARGUMENT}0025";

    /// <summary>
    /// The length of '{0}' must be {1} characters or fewer.
    /// </summary>
    [Description("The length of '{PropertyName}' must be {MaxLength} characters or fewer.")]
    public const string MAXIMUM_LENGTH_SIMPLE = $"{ARGUMENT}0026";

    /// <summary>
    /// '{0}' must be {1} characters in length.
    /// </summary>
    [Description("'{PropertyName}' must be {MaxLength} characters in length.")]
    public const string EXACT_LENGTH_SIMPLE = $"{ARGUMENT}0027";

    /// <summary>
    /// '{0}' must be between {1} and {2}.
    /// </summary>
    [Description("'{PropertyName}' must be between {From} and {To}.")]
    public const string INCLUSIVE_BETWEEN_SIMPLE = $"{ARGUMENT}0028";

    /// <summary>
    /// '{PropertyName}' cannot be Null or empty collection.
    /// </summary>
    [Description("'{0}' cannot be Null or empty collection.")]
    public const string NOT_NULL_AND_EMPTY_COLLECTION_VALIDATOR = $"{ARGUMENT}0029";

    /// <summary>
    /// '{PropertyName}' cannot be Null or whitespace.
    /// </summary>
    [Description("'{0}' cannot be Null or whitespace.")]
    public const string NOT_NULL_AND_WHITESPACE_VALIDATOR = $"{ARGUMENT}0030";

    /// <summary>
    /// '{PropertyName}' cannot contain {Content}.
    /// </summary>
    [Description("'{0}' cannot contain {1}.")]
    public const string NOT_CONTAIN_VALIDATOR = $"{ARGUMENT}0031";

    /// <summary>
    /// '{PropertyName}' must be greater than or equal to '{min}' and less than or equal to '{max}'.
    /// </summary>
    [Description("'{0}' must be greater than or equal to '{1}' and less than or equal to '{2}'.")]
    public const string OUT_OF_RANGE_VALIDATOR = $"{ARGUMENT}0032";

    #endregion

    #region BackgroundJob

    /// <summary>
    /// '{PropertyName}' is not a valid email address.
    /// </summary>
    [Description("No matching background task parameter type found, jobName: '{0}'")]
    public const string NOT_FIND_JOB_ARGS_BY_JOB_NAME = $"{BACKGROUND_JOB}0001";

    /// <summary>
    /// '{PropertyName}' is not a valid email address.
    /// </summary>
    [Description("No matching background task found, jobName: '{0}'")]
    public const string NOT_FIND_JOB_BY_JOB_NAME = $"{BACKGROUND_JOB}0002";

    /// <summary>
    /// '{PropertyName}' is not a valid email address.
    /// </summary>
    [Description("No matching background task found, jobType: '{0}'")]
    public const string NOT_FIND_JOB = $"{BACKGROUND_JOB}0003";

    #endregion

    #region Other

    /// <summary>
    /// Internal service error
    /// </summary>
    [Description("Internal service error")]
    public const string INTERNAL_SERVER_ERROR = $"{INTERNAL_SERVER}0001";

    #endregion

    private static readonly Dictionary<string, string?> _errorCodeMessageDictionary = new();

    static ExceptionErrorCode()
    {
        var classType = typeof(ExceptionErrorCode);
        var fields = classType.GetFields(BindingFlags.Static | BindingFlags.Public);
        foreach (var field in fields)
        {
            var errorMessage = AttributeUtils.GetDescriptionValueByField(field);

            _errorCodeMessageDictionary.Add(field.GetRawConstantValue()!.ToString()!, errorMessage);
        }
    }

    public static string? GetErrorMessage(string errorCode)
    {
        if (_errorCodeMessageDictionary.TryGetValue(errorCode, out string? errorMessage))
            return errorMessage;

        return null;
    }
}
