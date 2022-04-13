namespace Masa.Contrib.BasicAbility.Dcc.Tests;

public class CustomMemoryCacheClientFactory : IMemoryCacheClientFactory
{
    private readonly IMemoryCache _memoryCache;

    public CustomMemoryCacheClientFactory(IMemoryCache memoryCache) => _memoryCache = memoryCache;

    public MemoryCacheClient CreateClient(string name) => new(_memoryCache, null!, SubscribeKeyTypes.SpecificPrefix);
}
