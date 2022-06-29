using SqlSugar;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlSugar(this IServiceCollection services, string connectionString, DbType dbType = DbType.SqlServer)
        => services.AddSqlSugar(new SqlSugarScope(new ConnectionConfig
        {
            DbType = dbType,
            IsAutoCloseConnection = true,
            ConnectionString = connectionString
        }));


    public static IServiceCollection AddSqlSugar(this IServiceCollection services, SqlSugarScope sqlSugarClient)
        => services.AddSingleton<ISqlSugarClient>(sqlSugarClient);
}
