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
    /// Provides error message that I18N is not used
    /// </summary>
    protected string? ErrorMessage { get; set; }

    private object[] _parameters;

    public object[] Parameters => _parameters;

    private bool _initialize;

    private II18N? _i18N;

    internal II18N? I18N
    {
        get
        {
            TryInitialize();
            return _i18N;
        }
    }

    private II18N? _frameworkI18N;

    internal II18N? FrameworkI18N
    {
        get
        {
            TryInitialize();
            return _frameworkI18N;
        }
    }

    private bool _supportI18N;

    internal bool SupportI18N
    {
        get
        {
            TryInitialize();
            return _supportI18N;
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
        _frameworkI18N = MasaApp.GetService<II18N<MasaFrameworkResource>>();
        _i18N = MasaApp.GetService<II18N<DefaultResource>>();
        _supportI18N = _frameworkI18N != null;
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

    public MasaException(string errorCode, LogLevel? logLevel = null, params object[] parameters)
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
        if (!SupportI18N)
        {
            if (string.IsNullOrWhiteSpace(ErrorMessage))
                return Message;

            return string.Format(ErrorMessage, GetParameters());
        }

        if (ErrorCode!.StartsWith(Masa.BuildingBlocks.Data.Constants.ErrorCode.FRAMEWORK_PREFIX))
        {
            //The current framework frame exception
            return FrameworkI18N!.T(ErrorCode!, false, GetParameters()) ?? Message;
        }

        return I18N!.T(ErrorCode, false, GetParameters()) ?? Message;
    }

    protected virtual object[] GetParameters() => Parameters;

    public string? GetErrorMessage() => ErrorMessage;
}
