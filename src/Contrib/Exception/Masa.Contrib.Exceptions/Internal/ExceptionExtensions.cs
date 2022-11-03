// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Exceptions.Internal;

internal static class ExceptionExtensions
{
    public static int GetHttpStatusCode(this Exception exception)
    {
        if (exception is UserFriendlyException)
            return (int)MasaHttpStatusCode.UserFriendlyException;

        return (int)HttpStatusCode.InternalServerError;
    }

    public static string GetMessage(this MasaException masaException, II18N? frameworkI18N, II18N? i18N)
    {
        if (!string.IsNullOrWhiteSpace(masaException.ErrorCode))
        {
            if (!masaException.ErrorCode.StartsWith(ErrorCode.FRAMEWORK_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                if (i18N != null)
                    return i18N[masaException.ErrorCode, masaException.Parameters];

                if (masaException.ErrorMessage != null)
                    return string.Format(masaException.ErrorMessage, masaException.Parameters);

                return masaException.Message;
            }
            if (masaException is MasaArgumentException masaArgumentException)
            {
                if (frameworkI18N != null)
                    return frameworkI18N[masaException.ErrorCode, new List<object>()
                    {
                        masaArgumentException.ParamName!,
                        masaArgumentException.Parameters
                    }];
                if (masaArgumentException.ErrorMessage != null)
                    return string.Format(masaArgumentException.ErrorMessage, masaException.Parameters);

                return masaArgumentException.Message;
            }
            if (frameworkI18N != null)
                return frameworkI18N[masaException.ErrorCode, masaException.Parameters];

            if (masaException.ErrorMessage != null)
                return string.Format(masaException.ErrorMessage, masaException.Parameters);

            return masaException.Message;
        }
        return masaException.Message;
    }
}
