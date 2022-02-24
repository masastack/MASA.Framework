namespace Masa.Contrib.Dispatcher.Events.OnlyCancelHandler.Tests.EventHandlers;

public class UserEventHandler
{
    [EventHandler(IsCancel = true)]
    public void BindPhoneNumber(BindPhoneNumberEvent @event)
    {

    }
}
