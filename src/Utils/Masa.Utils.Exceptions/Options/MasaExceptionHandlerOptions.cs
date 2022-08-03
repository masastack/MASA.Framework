// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Exceptions;

public class MasaExceptionHandlerOptions
{
    public bool CatchAllException { get; set; } = true;

    public Action<MasaExceptionContext>? ExceptionHandler { get; set; }

    internal Type? MasaExceptionHandlerType { get; private set; }

    public void UseExceptionHanlder<TExceptionHanlder>() where TExceptionHanlder : IMasaExceptionHandler
    {
        MasaExceptionHandlerType = typeof(TExceptionHanlder);
    }
}
