namespace MASA.Contrib.Service.MinimalAPIs.Tests.Services;

public class CustomService : ServiceBase
{
    private int _times = 0;

    public CustomService(IServiceCollection services) : base(services)
    {
        _times++;
    }

    public int Test() => _times;
}
