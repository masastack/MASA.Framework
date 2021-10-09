namespace MASA.Contribs.Data.Contracts.EF;

public static class ServiceCollectionExtensions
{
    public static MasaDbContextOptionsBuilder UseSoftDelete(
        this MasaDbContextOptionsBuilder optionsBuilder, IServiceCollection services)
    {
        if (services.All(service => service.ServiceType != typeof(IUnitOfWork)))
        {
            throw new Exception("Please add UoW first.");
        }

        optionsBuilder.UseQueryFilterProvider<QueryFilterProvider>();
        optionsBuilder.UseSaveChangesFilter<SoftDeleteSaveChangesFilter>();

        return optionsBuilder;
    }
}
