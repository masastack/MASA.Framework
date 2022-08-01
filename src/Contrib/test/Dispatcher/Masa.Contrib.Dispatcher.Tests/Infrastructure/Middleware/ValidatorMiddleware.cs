﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Tests.Infrastructure.Middleware;

public class ValidatorMiddleware<TEvent> : Middleware<TEvent>
    where TEvent : notnull, IEvent
{
    private readonly IEnumerable<IValidator<TEvent>> _validators;

    public ValidatorMiddleware(IEnumerable<IValidator<TEvent>> validators)
    {
        _validators = validators;
    }

    public override async Task HandleAsync(TEvent action, EventHandlerDelegate next)
    {
        var failures = _validators
            .Select(v => v.Validate(action))
            .SelectMany(result => result.Errors)
            .Where(error => error != null)
            .ToList();

        if (failures.Any())
        {
            throw new ValidationException(failures.Select(x=>x.ErrorMessage).FirstOrDefault());
        }

        await next();
    }
}
