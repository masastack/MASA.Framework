namespace Masa.Contrib.Isolation.UoW.EF.Middleware;

public interface IIsolationMiddleware
{
    Task HandleAsync();
}
