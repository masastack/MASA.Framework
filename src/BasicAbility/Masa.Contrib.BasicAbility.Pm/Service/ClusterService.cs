namespace Masa.Contrib.BasicAbility.Pm.Service;

public class ClusterService : IClusterService
{
    private readonly ICallerProvider _callerProvider;

    public ClusterService(ICallerProvider callerProvider)
    {
        _callerProvider = callerProvider;
    }

    public async Task<ClusterDetailModel> GetAsync(int Id)
    {
        var requestUri = $"api/v1/cluster/{Id}";
        var result = await _callerProvider.GetAsync<ClusterDetailModel>(requestUri);

        return result ?? new();
    }

    public async Task<List<EnvironmentClusterModel>> GetEnvironmentClustersAsync()
    {
        var requestUri = $"api/v1/envClusters";
        var result = await _callerProvider.GetAsync<List<EnvironmentClusterModel>>(requestUri);

        return result ?? new();
    }

    public async Task<List<ClusterModel>> GetListAsync()
    {
        var requestUri = $"api/v1/cluster";
        var result = await _callerProvider.GetAsync<List<ClusterModel>>(requestUri);

        return result ?? new();
    }

    public async Task<List<ClusterModel>> GetListByEnvIdAsync(int envId)
    {
        var requestUri = $"api/v1/{envId}/cluster";
        var result = await _callerProvider.GetAsync<List<ClusterModel>>(requestUri);

        return result ?? new();
    }
}
