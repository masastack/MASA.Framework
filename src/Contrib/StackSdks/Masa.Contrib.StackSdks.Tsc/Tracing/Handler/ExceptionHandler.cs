// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Tracing.Handler;

public class ExceptionHandler
{
    public virtual void OnException(Activity activity, Exception exception)
    {
        if (exception != null)
        {
            activity.SetTag(OpenTelemetryAttributeName.Exception.MESSAGE, exception.Message);
            activity.SetTag(OpenTelemetryAttributeName.Exception.STACKTRACE, exception.ToString());
        }
    }

    public virtual bool IsRecordException { get; set; } = true;
}
