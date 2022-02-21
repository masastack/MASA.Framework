namespace MASA.BuildingBlocks.Configuration;
public interface IConfigurationApiManage
{
    Task UpdateAsync(string environment, string cluster, string appId, string configObject, object value);
}
