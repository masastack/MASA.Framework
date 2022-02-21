namespace MASA.BuildingBlocks.Service.MinimalAPIs;
public interface IService
{
    WebApplication App { get; }

    IServiceCollection Services { get; }

    TService? GetService<TService>();

    TService GetRequiredService<TService>() where TService : notnull;
}
