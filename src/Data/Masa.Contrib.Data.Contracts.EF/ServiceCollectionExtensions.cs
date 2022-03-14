namespace Masa.Contrib.Data.Contracts.EF;

public static class ServiceCollectionExtensions
{
    [Obsolete("Please use optionsBuilder.UseSoftDelete()")]
    public static MasaDbContextOptionsBuilder UseSoftDelete(
        this MasaDbContextOptionsBuilder masaDbContextOptionsBuilder,
        IServiceCollection services) => masaDbContextOptionsBuilder.UseSoftDelete();

    public static MasaDbContextOptionsBuilder UseSoftDelete(
        this MasaDbContextOptionsBuilder masaDbContextOptionsBuilder)
    {
        if (masaDbContextOptionsBuilder.Services.Any(s => s.ImplementationType == typeof(ContractsFilter))) return masaDbContextOptionsBuilder;
        masaDbContextOptionsBuilder.Services.AddSingleton<ContractsFilter>();

        if (masaDbContextOptionsBuilder.Services.All(service => service.ServiceType != typeof(IUnitOfWork)))
            throw new Exception("Please add UoW first.");

        masaDbContextOptionsBuilder.UseQueryFilterProvider<QueryFilterProvider>()
            .UseSaveChangesFilter<SoftDeleteSaveChangesFilter>();

        return masaDbContextOptionsBuilder;
    }

    private class ContractsFilter
    {
    }
}
