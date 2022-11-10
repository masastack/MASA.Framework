// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

[Serializable]
public class MasaArgumentException : MasaException
{
    protected string? ParamName { get; }

    public MasaArgumentException(string message)
        : base(message)
    {
    }

    public MasaArgumentException(string message, string paramName)
        : base(message)
    {
        ParamName = paramName;
    }

    public MasaArgumentException(string? paramName, string errorCode, params object[] parameters)
        : this((Exception?)null, errorCode, parameters)
    {
        ParamName = paramName;
    }

    public MasaArgumentException(Exception? innerException, string errorCode, params object[] parameters)
        : base(innerException, errorCode, parameters)
    {
    }

    public MasaArgumentException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public MasaArgumentException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {
    }

    public static void ThrowIfNullOrEmptyCollection<T>(IEnumerable<T>? arguments,
        [CallerArgumentExpression("arguments")]
        string? paramName = null)
    {
        ThrowIf(arguments is null || !arguments.Any(),
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.NOT_NULL_AND_EMPTY_COLLECTION_VALIDATOR);
    }

    public static void ThrowIfNull(object? argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIf(argument is null,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.NOT_NULL_VALIDATOR);
    }

    public static void ThrowIfNullOrEmpty(object? argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIf(string.IsNullOrEmpty(argument?.ToString()),
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.NOT_NULL_AND_EMPTY_VALIDATOR);
    }

    public static void ThrowIfNullOrWhiteSpace(object? argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIf(string.IsNullOrWhiteSpace(argument?.ToString()),
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.NOT_NULL_AND_WHITESPACE_VALIDATOR);
    }

    public static void ThrowIfGreaterThan<T>(T argument,
        T maxValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(maxValue) > 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.LESS_THAN_OR_EQUAL_VALIDATOR,
            maxValue);
    }

    public static void ThrowIfGreaterThanOrEqual<T>(T argument,
        T maxValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(maxValue) >= 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.LESS_THAN_VALIDATOR,
            maxValue);
    }

    public static void ThrowIfLessThan<T>(T argument,
        T minValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(minValue) < 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.GREATER_THAN_OR_EQUAL_VALIDATOR,
            minValue);
    }

    public static void ThrowIfLessThanOrEqual<T>(T argument,
        T minValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(minValue) <= 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.GREATER_THAN_VALIDATOR,
            minValue);
    }

    public static void ThrowIfOutOfRange<T>(T argument,
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
                Masa.BuildingBlocks.Data.Constants.ErrorCode.NOT_CONTAIN_VALIDATOR
            );
    }

    public static void ThrowIf(bool condition, string? paramName, string errorCode, params object[] parameters)
    {
        if (condition) Throw(paramName, errorCode, Masa.BuildingBlocks.Data.Constants.ErrorCode.GetErrorMessage(errorCode), parameters);
    }

    public static void ThrowIf(bool condition, string? paramName, string errorCode, string? errorMessage, params object[] parameters)
    {
        if (condition) Throw(paramName, errorCode, errorMessage, parameters);
    }

    [DoesNotReturn]
    private static void Throw(string? paramName, string errorCode, string? errorMessage, params object[] parameters) =>
        throw new MasaArgumentException(paramName, errorCode, parameters)
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
