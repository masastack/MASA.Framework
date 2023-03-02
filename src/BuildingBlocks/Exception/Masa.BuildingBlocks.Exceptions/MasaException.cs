// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

[Serializable]
public class MasaException : Exception
{
    private LogLevel? _logLevel;
    public LogLevel? LogLevel => _logLevel;

    private string? _errorCode;
    public string? ErrorCode => _errorCode;

    /// <summary>
    /// Provides error message that I18n is not used
    /// </summary>
    protected string? ErrorMessage { get; set; }

    private object[] _parameters;

    public object[] Parameters => _parameters;

    private bool _initialize;

    private II18n? _i18n;

    internal II18n? I18n
    {
        get
        {
            TryInitialize();
            return _i18n;
        }
    }

    private II18n? _frameworkI18n;

    internal II18n? FrameworkI18n
    {
        get
        {
            TryInitialize();
            return _frameworkI18n;
        }
    }

    private bool _supportI18n;

    internal bool SupportI18n
    {
        get
        {
            TryInitialize();
            return _supportI18n;
        }
    }

    private void TryInitialize()
    {
        if (_initialize)
            return;

        Initialize();
    }

    private void Initialize()
    {
        _frameworkI18n = MasaApp.GetService<II18n<MasaFrameworkResource>>();
        _i18n = MasaApp.GetService<II18n<DefaultResource>>();
        _supportI18n = _frameworkI18n != null;
        _initialize = true;
    }

    public MasaException()
    {
    }

    public MasaException(string message, LogLevel? logLevel = null)
        : base(message)
    {
        _logLevel = logLevel;
    }

    public MasaException(string errorCode, LogLevel? logLevel, params object[] parameters)
        : this(null, errorCode, logLevel, parameters)
    {
    }

    public MasaException(Exception? innerException, string errorCode, LogLevel? logLevel = null, params object[] parameters)
        : base(null, innerException)
    {
        _errorCode = errorCode;
        _parameters = parameters;
        _logLevel = logLevel;
    }

    public MasaException(string message, Exception? innerException, LogLevel? logLevel = null)
        : base(message, innerException)
    {
        _logLevel = logLevel;
    }

    protected MasaException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {
    }

    public string GetLocalizedMessage()
    {
        if (string.IsNullOrWhiteSpace(ErrorCode))
            return Message;

        return GetLocalizedMessageExecuting();
    }

    protected virtual string GetLocalizedMessageExecuting()
    {
        if (!SupportI18n)
        {
            if (string.IsNullOrWhiteSpace(ErrorMessage))
                return Message;

            return string.Format(ErrorMessage, GetParameters());
        }

        if (ErrorCode!.StartsWith(Masa.BuildingBlocks.Data.Constants.ExceptionErrorCode.FRAMEWORK_PREFIX))
        {
            //The current framework frame exception
            return FrameworkI18n!.T(ErrorCode!, false, GetParameters()) ?? Message;
        }

        return I18n!.T(ErrorCode, false, GetParameters()) ?? Message;
    }

    protected virtual object[] GetParameters() => Parameters;

    public string? GetErrorMessage() => ErrorMessage;
}
