namespace MASA.Contrib.Dispatcher.InMemory.OnlyCancelHandler.Tests.EventHandlers;

public class UserEventHandler
{
    [EventHandler(IsCancel = true)]
    public void BindMobile(BindMobileEvent @event)
    {

    }
}
