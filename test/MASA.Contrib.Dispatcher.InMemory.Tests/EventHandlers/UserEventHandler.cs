namespace MASA.Contrib.Dispatcher.InMemory.Tests.EventHandlers;

public class UserEventHandler : IEventHandler<EditUserEvent>
{
    [EventHandler(10, true, 1, FailureLevels = Enums.FailureLevels.ThrowAndCancel)]
    public Task HandleAsync(EditUserEvent @event)
    {
        throw new NotSupportedException("users cannot be modified");
    }

    /// <summary>
    /// This CancelHandler is not the same as EventHandler in Saga mode, so a different order can be used
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    [EventHandler(20, true, 1, FailureLevels = Enums.FailureLevels.Ignore, IsCancel = true)]
    public Task CancelAsync(EditUserEvent @event)
    {
        throw new NotSupportedException("edit users do not support cancellation");
    }

    [EventHandler(10, true)]
    public void ForgotPassword(ForgotPasswordEvent @event)
    {
        throw new Exception("Password retrieval is not supported");
    }
}