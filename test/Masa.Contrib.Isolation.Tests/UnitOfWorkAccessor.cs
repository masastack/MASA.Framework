namespace Masa.Contrib.Isolation.Tests;

public class UnitOfWorkAccessor: IUnitOfWorkAccessor
{
    public MasaDbContextConfigurationOptions? CurrentDbContextOptions { get; set; }
}
