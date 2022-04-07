
using Masa.BuildingBlocks.BasicAbility.Pm.Service;

namespace Masa.Contrib.BasicAbility.Pm
{
    public class PmClient : IPmClient
    {
        public PmClient(ICallerProvider callerProvider)
        {
            EnvironmentService = new EnvironmentService(callerProvider);
            ClusterService = new ClusterService(callerProvider);
            ProjectService = new ProjectService(callerProvider);
            AppService = new AppService(callerProvider);
        }

        public IProjectService ProjectService { get; init; }

        public IEnvironmentService EnvironmentService { get; init; }

        public IClusterService ClusterService { get; init; }

        public IAppService AppService { get; init; }
    }
}
