// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Internal.Middleware;

internal class ExceptionEventMiddleware<TEvent> : EventMiddleware<TEvent>
    where TEvent : IEvent
{
    private readonly Lazy<IExecuteProvider> _executeProviderLazy;
    private IExecuteProvider ExecuteProvider => _executeProviderLazy.Value;

    private readonly Lazy<ILocalEventBus> _localEventBusLazy;
    private ILocalEventBus LocalEventBus => _localEventBusLazy.Value;

    public override bool SupportRecursive => false;

    public ExceptionEventMiddleware(IServiceProvider serviceProvider)
    {
        _executeProviderLazy = new Lazy<IExecuteProvider>(serviceProvider.GetRequiredService<IExecuteProvider>);
        _localEventBusLazy = new Lazy<ILocalEventBus>(serviceProvider.GetRequiredService<ILocalEventBus>);
    }

    public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        try
        {
            ExecuteProvider.Initialize();

            await next();
        }
        catch (Exception exception)
        {
            var executeResult = ExecuteProvider.ExecuteResult;
            if (executeResult.Status == ExecuteStatus.InProgress)
            {
                // An error occurred before the execution of the EventHandler, no need to roll back
                executeResult.Status = ExecuteStatus.Failed;
                ExecuteProvider.SetExecuteResult(executeResult);
                exception.ThrowException();
                return;
            }

            if (ExecuteProvider.ExecuteResult.Status == ExecuteStatus.Succeed)
            {
                executeResult.Exception = exception;

                //The Handler is successful, but the execution of the middleware is abnormal

                await LocalEventBus.ExecuteAllCancelHandlerAsync(@event, default);
            }

            executeResult.Exception!.ThrowException();
        }
        finally
        {
            ExecuteProvider.ResetTimer();
        }
    }
}
