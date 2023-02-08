// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Middleware;

internal class ValidatorEventMiddleware<TEvent> : EventMiddleware<TEvent>
        where TEvent : notnull, IEvent
{
    private readonly ILogger<ValidatorEventMiddleware<TEvent>> _logger;
    private readonly IEnumerable<IValidator<TEvent>> _validators;

    public ValidatorEventMiddleware(IEnumerable<IValidator<TEvent>> validators, ILogger<ValidatorEventMiddleware<TEvent>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        var typeName = @event.GetType().FullName;

        _logger.LogInformation("----- Validating command {CommandType}", typeName);

        var failures = _validators
            .Select(v => v.Validate(@event))
            .SelectMany(result => result.Errors)
            .Where(error => error != null)
            .ToList();

        if (failures.Any())
        {
            _logger.LogError("Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}", typeName, @event, failures);
            throw new ValidationException("Validation exception", failures);
        }

        await next();
    }
}
