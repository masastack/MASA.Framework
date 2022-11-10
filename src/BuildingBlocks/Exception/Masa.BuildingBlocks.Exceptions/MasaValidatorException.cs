// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

[Serializable]
public class MasaValidatorException : MasaArgumentException
{
    public MasaValidatorException(string message)
        : base(message)
    {
    }

    public MasaValidatorException(string? paramName, string errorCode, params object[] parameters)
        : base(paramName, errorCode, parameters)
    {

    }

    public MasaValidatorException(Exception? innerException, string errorCode, params object[] parameters)
        : base(innerException, errorCode, parameters)
    {
    }

    public MasaValidatorException(params ValidationModel[] validationModels)
        : base(FormatMessage(validationModels))
    {

    }

    public MasaValidatorException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public MasaValidatorException(SerializationInfo serializationInfo, StreamingContext context)
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

    public new static void ThrowIfNullOrEmptyCollection<T>(IEnumerable<T>? arguments,
        [CallerArgumentExpression("arguments")]
        string? paramName = null)
    {
        ThrowIf(arguments is null || !arguments.Any(),
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.NOT_NULL_AND_EMPTY_COLLECTION_VALIDATOR);
    }

    public new static void ThrowIfNull(
        object? argument,
        [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIf(argument is null,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.NOT_NULL_VALIDATOR);
    }

    public new static void ThrowIfNullOrEmpty(
        object? argument,
        [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIf(string.IsNullOrEmpty(argument?.ToString()),
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.NOT_NULL_AND_EMPTY_VALIDATOR);
    }

    public new static void ThrowIfNullOrWhiteSpace(
        object? argument,
        [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIf(string.IsNullOrWhiteSpace(argument?.ToString()),
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.NOT_NULL_AND_WHITESPACE_VALIDATOR);
    }

    public new static void ThrowIfGreaterThan<T>(T argument,
        T maxValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(maxValue) > 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.LESS_THAN_OR_EQUAL_VALIDATOR,
            maxValue);
    }

    public new static void ThrowIfGreaterThanOrEqual<T>(T argument,
        T maxValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(maxValue) >= 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.LESS_THAN_VALIDATOR,
            maxValue);
    }

    public new static void ThrowIfLessThan<T>(T argument,
        T minValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(minValue) < 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.GREATER_THAN_OR_EQUAL_VALIDATOR,
            minValue);
    }

    public new static void ThrowIfLessThanOrEqual<T>(T argument,
        T minValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(minValue) <= 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.GREATER_THAN_VALIDATOR,
            minValue);
    }

    public new static void ThrowIfOutOfRange<T>(T argument,
        T minValue,
        T maxValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(minValue) < 0 || argument.CompareTo(maxValue) > 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.OUT_OF_RANGE_VALIDATOR,
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
                Masa.BuildingBlocks.Data.Constants.ErrorCode.NOT_CONTAIN_VALIDATOR
            );
    }

    public new static void ThrowIf(
        bool condition,
        string? paramName,
        string errorCode,
        params object[] parameters)
    {
        if (condition) Throw(paramName, errorCode, Masa.BuildingBlocks.Data.Constants.ErrorCode.GetErrorMessage(errorCode), parameters);
    }

    public new static void ThrowIf(
        bool condition,
        string? paramName,
        string errorCode,
        string? errorMessage,
        params object[] parameters)
    {
        if (condition) Throw(paramName, errorCode, errorMessage, parameters);
    }

    [DoesNotReturn]
    private static void Throw(
        string? paramName,
        string errorCode,
        string? errorMessage,
        params object[] parameters)
        => throw new MasaValidatorException(paramName, errorCode, parameters)
        {
            ErrorMessage = errorMessage
        };

    protected override string GetLocalizedMessageExecuting()
    {
        string message;
        if (!SupportI18N)
        {
            message = string.IsNullOrWhiteSpace(ErrorMessage) ? Message : string.Format(ErrorMessage, GetParameters());
        }

        else if (ErrorCode!.StartsWith(Masa.BuildingBlocks.Data.Constants.ErrorCode.FRAMEWORK_PREFIX))
        {
            //The current framework frame exception
            message = FrameworkI18N!.T(ErrorCode!, false, GetParameters()) ?? Message;
        }
        else
        {
            message = I18N!.T(ErrorCode, false, GetParameters()) ?? Message;
        }

        return FormatMessage(new ValidationModel(ParamName!, message));
    }
}
