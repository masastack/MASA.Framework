namespace Masa.Contrib.Isolation.UoW.EF.Web.Tests.Events;

public record AddRoleEvent(string Name,int Quantity) : Event
{
}
