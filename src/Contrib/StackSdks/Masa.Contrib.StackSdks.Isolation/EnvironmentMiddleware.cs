// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Isolation
{
    internal class EnvironmentMiddleware : IMiddleware
    {
        readonly IMultiEnvironmentContext _multiEnvironmentContext;
        readonly ILogger<EnvironmentMiddleware> _logger;

        public EnvironmentMiddleware(IMultiEnvironmentContext multiEnvironmentContext,
            ILogger<EnvironmentMiddleware> logger)
        {
            _multiEnvironmentContext = multiEnvironmentContext;
            _logger = logger;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _logger.LogDebug("----- Current Environment Is [{0}] -----", _multiEnvironmentContext.CurrentEnvironment);
            //context.Items.Add(IsolationConsts.ENVIRONMENT, _multiEnvironmentContext.CurrentEnvironment);
            return next(context);
        }
    }
}
