namespace Masa.Contrib.Isolation.MultiTenancy;

public static class IsolationBuilderExtensions
{
    public static IIsolationBuilder UseMultiTenancy(this IIsolationBuilder isolationBuilder) => isolationBuilder.UseMultiTenancy<Guid>();

    public static IIsolationBuilder UseMultiTenancy<TKey>(this IIsolationBuilder isolationBuilder) where TKey : IComparable
    {
        isolationBuilder.Services.TryAddSingleton<IConvertProvider, ConvertProvider>();
        isolationBuilder.Services.TryAddEnumerable(new ServiceDescriptor(typeof(ISaveChangesFilter), typeof(TenancySaveChangesFilter<TKey>), ServiceLifetime.Scoped));
        isolationBuilder.Services.TryAddScoped<TenantContext>();
        isolationBuilder.Services.TryAddScoped(typeof(ITenantContext), serviceProvider => serviceProvider.GetRequiredService<TenantContext>());
        isolationBuilder.Services.TryAddScoped(typeof(ITenantSetter), serviceProvider => serviceProvider.GetRequiredService<TenantContext>());
        return isolationBuilder;
    }
}
