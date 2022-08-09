// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

public class MasaExceptionHandlingOptions
{
    public bool CatchAllException { get; set; } = true;

    public Func<Exception, (Exception? OverrideException, bool ExceptionHandled)>? CustomExceptionHandler { get; set; }
}
