namespace Masa.Contrib.Dispatcher.Events.Tests.EventHandlers;

public class RegisterUserEventHandler
{
    [EventHandler]
    public void RegisterUser(RegisterUserEvent registerUserEvent)
    {
        throw new NotSupportedException();
    }
}

