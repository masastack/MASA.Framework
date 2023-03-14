// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

[Serializable]
public class MasaValidatorException : MasaArgumentException
{
    public MasaValidatorException(string message, LogLevel? logLevel = null)
        : base(message, logLevel)
    {
    }

    public MasaValidatorException(string? paramName, string errorCode, LogLevel? logLevel = null, params object[] parameters)
        : base(paramName, errorCode, logLevel, parameters)
    {

    }

    public MasaValidatorException(Exception? innerException, string errorCode, LogLevel? logLevel = null, params object[] parameters)
        : base(innerException, errorCode, logLevel, parameters)
    {
    }

    public MasaValidatorException(params ValidationModel[] validationModels)
        : base(FormatMessage(validationModels), Microsoft.Extensions.Logging.LogLevel.Error)
    {

    }

    public MasaValidatorException(LogLevel? logLevel = null, params ValidationModel[] validationModels)
        : base(FormatMessage(validationModels), logLevel)
    {

    }

    public MasaValidatorException(string message, Exception? innerException, LogLevel? logLevel = null)
        : base(message, innerException, logLevel)
    {
    }

    protected MasaValidatorException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {
    }

    private static string FormatMessage(params ValidationModel[] models)
        => FormatMessage(models.ToList());

    private static string FormatMessage(IEnumerable<ValidationModel> models)
    {
        var stringBuilder = new Text.StringBuilder();
        stringBuilder.AppendLine("Validation failed: ");
        foreach (var model in models)
        {
            stringBuilder.AppendLine($"-- {model.Name}: {model.Message} Severity: {model.Level.ToString()}");
        }
        return stringBuilder.ToString();
    }

    public new static void ThrowIfNullOrEmptyCollection<T>([NotNull] IEnumerable<T>? arguments,
        [CallerArgumentExpression("arguments")]
        string? paramName = null)
    {
        ThrowIf(arguments is null || !arguments.Any(),
            paramName,
            Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.NOT_NULL_AND_EMPTY_COLLECTION_VALIDATOR);
    }

    public new static void ThrowIfNull(
        [NotNull] object? argument,
        [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIf(argument is null,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.NOT_NULL_VALIDATOR);
    }

    public new static void ThrowIfNullOrEmpty(
        object? argument,
        [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIf(string.IsNullOrEmpty(argument?.ToString()),
            paramName,
            Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.NOT_NULL_AND_EMPTY_VALIDATOR);
    }

    public new static void ThrowIfNullOrWhiteSpace(
        [NotNull] object? argument,
        [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIf(string.IsNullOrWhiteSpace(argument?.ToString()),
            paramName,
            Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.NOT_NULL_AND_WHITESPACE_VALIDATOR);
    }

    public new static void ThrowIfGreaterThan<T>(T argument,
        T maxValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(maxValue) > 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.LESS_THAN_OR_EQUAL_VALIDATOR,
            null,
            maxValue);
    }

    public new static void ThrowIfGreaterThanOrEqual<T>(T argument,
        T maxValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(maxValue) >= 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.LESS_THAN_VALIDATOR,
            null,
            maxValue);
    }

    public new static void ThrowIfLessThan<T>(T argument,
        T minValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(minValue) < 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.GREATER_THAN_OR_EQUAL_VALIDATOR,
            null,
            minValue);
    }

    public new static void ThrowIfLessThanOrEqual<T>(T argument,
        T minValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(minValue) <= 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.GREATER_THAN_VALIDATOR,
            null,
            minValue);
    }

    public new static void ThrowIfOutOfRange<T>(T argument,
        T minValue,
        T maxValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(minValue) < 0 || argument.CompareTo(maxValue) > 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.OUT_OF_RANGE_VALIDATOR,
            null,
            minValue,
            maxValue);
    }

    public new static void ThrowIfContain(string? argument,
        string parameter,
        [CallerArgumentExpression("argument")] string? paramName = null)
        => ThrowIfContain(argument, parameter, StringComparison.OrdinalIgnoreCase, paramName);

    public new static void ThrowIfContain(string? argument,
        string parameter,
        StringComparison stringComparison,
        [CallerArgumentExpression("argument")] string? paramName = null)
    {
        if (argument != null)
            ThrowIf(argument.Contains(parameter, stringComparison),
                paramName,
                Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.NOT_CONTAIN_VALIDATOR
            );
    }

    public new static void ThrowIf(
        [DoesNotReturnIf(true)] bool condition,
        string? paramName,
        string errorCode,
        LogLevel? logLevel = null,
        params object[] parameters)
    {
        if (condition)
            Throw(paramName, errorCode, Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.GetErrorMessage(errorCode), logLevel, parameters);
    }

    public new static void ThrowIf(
        [DoesNotReturnIf(true)] bool condition,
        string? paramName,
        string errorCode,
        string? errorMessage,
        LogLevel? logLevel = null,
        params object[] parameters)
    {
        if (condition) Throw(paramName, errorCode, errorMessage, logLevel, parameters);
    }

    [DoesNotReturn]
    private static void Throw(
        string? paramName,
        string errorCode,
        string? errorMessage,
        LogLevel? logLevel,
        params object[] parameters)
        => throw new MasaValidatorException(paramName, errorCode, logLevel, parameters)
        {
            ErrorMessage = errorMessage
        };

    protected override string GetLocalizedMessageExecuting()
    {
        string message;
        if (!SupportI18n)
        {
            message = string.IsNullOrWhiteSpace(ErrorMessage) ? Message : string.Format(ErrorMessage, GetParameters());
        }

        else if (ErrorCode!.StartsWith(Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.FRAMEWORK_PREFIX))
        {
            //The current framework frame exception
            message = FrameworkI18n!.T(ErrorCode!, false, GetParameters()) ?? Message;
        }
        else
        {
            message = I18n!.T(ErrorCode, false, GetParameters()) ?? Message;
        }

        return FormatMessage(new ValidationModel(ParamName!, message));
    }
}
