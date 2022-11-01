// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

public class MasaArgumentException : MasaException2
{
    public string? ParamName { get; }

    public MasaArgumentException(string message)
        : base(message)
    {
    }

    public MasaArgumentException(string message, string paramName)
        : base(message)
    {
        ParamName = paramName;
    }

    public MasaArgumentException(string paramName, params object[] parameters)
        : this(paramName, Masa.BuildingBlocks.Data.Constants.ErrorCode.ARGUMENT_ERROR, parameters)
    {
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

    public static void ThrowIfNull(object? argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIf(argument is null,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.ARGUMENT_NULL);
    }

    public static void ThrowIfNullOrEmpty(object? argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIf(string.IsNullOrEmpty(argument?.ToString()),
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.ARGUMENT_NULL_OR_EMPTY);
    }

    public static void ThrowIfNullOrWhiteSpace(object? argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIf(argument is null,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.ARGUMENT_NULL_OR_WHITE_SPACE);
    }

    public static void ThrowIfGreaterThan<T>(T argument,
        T maxValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(maxValue) > 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.ARGUMENT_LESS_THAN_OR_EQUAL,
            maxValue);
    }

    public static void ThrowIfGreaterThanOrEqual<T>(T argument,
        T maxValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(maxValue) >= 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.ARGUMENT_LESS_THAN,
            maxValue);
    }

    public static void ThrowIfLessThan<T>(T argument,
        T minValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(minValue) < 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.ARGUMENT_GREATER_THAN_OR_EQUAL,
            minValue);
    }

    public static void ThrowIfLessThanOrEqual<T>(T argument,
        T minValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(minValue) <= 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.ARGUMENT_GREATER_THAN,
            minValue);
    }

    public static void ThrowIfOutOfRange<T>(T argument,
        T minValue,
        T maxValue,
        [CallerArgumentExpression("argument")] string? paramName = null) where T : IComparable
    {
        ThrowIf(argument.CompareTo(minValue) < 0 || argument.CompareTo(maxValue) > 0,
            paramName,
            Masa.BuildingBlocks.Data.Constants.ErrorCode.ARGUMENT_OUT_OF_RANGE,
            minValue,
            maxValue);
    }

    public static void ThrowIf(bool condition, string? paramName, string errorCode, params object[] parameters)
    {
        if (condition) Throw(paramName, errorCode, parameters);
    }

    [DoesNotReturn]
    private static void Throw(string? paramName, string errorCode, params object[] parameters) =>
        throw new MasaArgumentException(paramName, errorCode, parameters);
}
