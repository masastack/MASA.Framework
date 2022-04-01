namespace Masa.Contrib.Isolation.UoW.EF.Web.Tests.Events;

public record RegisterUserEvent(string Account,string Password) : Event
{
}
