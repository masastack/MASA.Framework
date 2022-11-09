// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

[Serializable]
public class MasaException : Exception
{
    public virtual LogLevel? LogLevel { get; set; }

    public string? ErrorCode { get; private set; }

    /// <summary>
    /// Provides error message that I18n is not used
    /// </summary>
    public string? ErrorMessage { get; set; }

    public object[] Parameters { get; private set; }

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

    public MasaException(string message)
        : base(message)
    {
    }

    public MasaException(string errorCode, params object[] parameters)
        : this(null, errorCode, parameters)
    {
    }

    public MasaException(Exception? innerException, string errorCode, params object[] parameters)
        : base(null, innerException)
    {
        ErrorCode = errorCode;
        Parameters = parameters;
    }

    public MasaException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public MasaException(SerializationInfo serializationInfo, StreamingContext context)
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
}
