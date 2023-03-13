// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

[Serializable]
public class MasaArgumentException : MasaException
{
    private string? _paramName;
    protected string? ParamName => _paramName;

    public MasaArgumentException(
        string message,
        LogLevel? logLevel = null)
        : base(message, logLevel)
    {
    }

    public MasaArgumentException(
        string message,
        string paramName,
        LogLevel? logLevel = null)
        : base(message, logLevel)
    {
        _paramName = paramName;
    }

    public MasaArgumentException(
        string? paramName,
        string errorCode,
        LogLevel? logLevel = null,
        params object[] parameters)
        : this((Exception?)null, errorCode, logLevel, parameters)
    {
        _paramName = paramName;
    }

    public MasaArgumentException(
        Exception? innerException,
        string errorCode,
        LogLevel? logLevel = null,
        params object[] parameters)
        : base(innerException, errorCode, logLevel, parameters)
    {
    }

    public MasaArgumentException(string message, Exception? innerException, LogLevel? logLevel = null)
        : base(message, innerException, logLevel)
    {
    }

    protected MasaArgumentException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {
    }

    public static void ThrowIfNullOrEmptyCollection<T>([NotNull] IEnumerable<T>? arguments,
        [CallerArgumentExpression("arguments")]
        string? paramName = null)
    {
        ThrowIf(arguments is null || !arguments.Any(),
            paramName,
            Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.NOT_NULL_AND_EMPTY_COLLECTION_VALIDATOR);
    }

    public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIf(argument is null,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.NOT_NULL_VALIDATOR);
    }

    public static void ThrowIfNullOrEmpty([NotNull] object? argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIf(string.IsNullOrEmpty(argument?.ToString()),
            paramName,
            Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.NOT_NULL_AND_EMPTY_VALIDATOR);
    }

    public static void ThrowIfNullOrWhiteSpace([NotNull] object? argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIf(string.IsNullOrWhiteSpace(argument?.ToString()),
            paramName,
            Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.NOT_NULL_AND_WHITESPACE_VALIDATOR);
    }

    public static void ThrowIfGreaterThan<T>(T argument,
        T maxValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(maxValue) > 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.LESS_THAN_OR_EQUAL_VALIDATOR,
            null,
            maxValue);
    }

    public static void ThrowIfGreaterThanOrEqual<T>(T argument,
        T maxValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(maxValue) >= 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.LESS_THAN_VALIDATOR,
            null,
            maxValue);
    }

    public static void ThrowIfLessThan<T>(T argument,
        T minValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(minValue) < 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.GREATER_THAN_OR_EQUAL_VALIDATOR,
            null,
            minValue);
    }

    public static void ThrowIfLessThanOrEqual<T>(T argument,
        T minValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(minValue) <= 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.GREATER_THAN_VALIDATOR,
            null,
            minValue);
    }

    public static void ThrowIfOutOfRange<T>(T argument,
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

    public static void ThrowIfContain(string? argument,
        string parameter,
        [CallerArgumentExpression("argument")] string? paramName = null)
        => ThrowIfContain(argument, parameter, StringComparison.OrdinalIgnoreCase, paramName);

    public static void ThrowIfContain(string? argument,
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

    public static void ThrowIf(
        [DoesNotReturnIf(true)] bool condition,
        string? paramName,
        string errorCode,
        LogLevel? logLevel = null,
        params object[] parameters)
    {
        if (condition)
            Throw(paramName, errorCode, Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.GetErrorMessage(errorCode), logLevel, parameters);
    }

    public static void ThrowIf(
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
        params object[] parameters) =>
        throw new MasaArgumentException(paramName, errorCode, logLevel, parameters)
        {
            ErrorMessage = errorMessage
        };

    protected override object[] GetParameters()
    {
        var parameters = new List<object>()
        {
            ParamName!
        };
        parameters.AddRange(Parameters);
        return parameters.ToArray();
    }
}
