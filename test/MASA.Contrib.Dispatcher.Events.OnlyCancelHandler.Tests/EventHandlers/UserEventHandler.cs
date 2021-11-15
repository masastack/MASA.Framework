namespace MASA.Contrib.Dispatcher.Events.OnlyCancelHandler.Tests.EventHandlers;

public class UserEventHandler
{
    [EventHandler(IsCancel = true)]
    public void BindMobile(BindMobileEvent @event)
    {

    }
}
