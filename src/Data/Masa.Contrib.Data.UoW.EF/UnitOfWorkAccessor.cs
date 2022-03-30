namespace Masa.Contrib.Data.UoW.EF;

public class UnitOfWorkAccessor : IUnitOfWorkAccessor
{
    public MasaDbContextConfigurationOptions? CurrentDbContextOptions { get; set; }
}
