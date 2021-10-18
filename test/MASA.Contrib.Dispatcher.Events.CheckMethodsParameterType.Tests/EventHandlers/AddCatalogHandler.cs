using MASA.Contrib.Dispatcher.Events.CheckMethodsParameterType.Tests.Events;

namespace MASA.Contrib.Dispatcher.Events.CheckMethodsParameterType.Tests.EventHandlers;

public class AddCatalogHandler
{
    /// <summary>
    /// The method name of this method can be named according to the actual business
    /// but we recommend HandlerAsync or CancelAsync if the business is single
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    [EventHandler]
    public Task<string> HandleAsync(AddCatalogEvent @event)
    {
        return Task.FromResult("success");
    }
}
