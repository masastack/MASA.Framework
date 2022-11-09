// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Exceptions.Internal;

internal static class ExceptionExtensions
{
    public static int GetHttpStatusCode(this Exception exception)
    {
        if (exception is UserFriendlyException)
            return (int)MasaHttpStatusCode.UserFriendlyException;

        if (exception is MasaValidatorException)
            return (int)MasaHttpStatusCode.ValidatorException;

        return (int)HttpStatusCode.InternalServerError;
    }
}
