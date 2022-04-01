namespace Masa.Contrib.Isolation.UoW.EF;

public static class IsolationBuilderExtensions
{
    public static TApplicationBuilder UseIsolation<TApplicationBuilder>(this TApplicationBuilder app) where TApplicationBuilder : IApplicationBuilder
    {
        app.UseMiddleware<IsolationMiddleware>();
        return app;
    }
}
