namespace Masa.Contrib.Data.Contracts.EF;

public static class ServiceCollectionExtensions
{
    public static MasaDbContextOptionsBuilder UseSoftDelete(
        this MasaDbContextOptionsBuilder optionsBuilder, IServiceCollection services)
    {
        if (services.Any(s => s.ImplementationType == typeof(ContractsFilter))) return optionsBuilder;
        services.AddSingleton<ContractsFilter>();

        if (services.All(service => service.ServiceType != typeof(IUnitOfWork)))
            throw new Exception("Please add UoW first.");

        optionsBuilder.UseQueryFilterProvider<QueryFilterProvider>()
            .UseSaveChangesFilter<SoftDeleteSaveChangesFilter>();

        return optionsBuilder;
    }

    private class ContractsFilter
    {
    }
}
