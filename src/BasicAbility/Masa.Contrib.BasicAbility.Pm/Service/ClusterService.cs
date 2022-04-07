namespace Masa.Contrib.BasicAbility.Pm.Service
{
    public class ClusterService : IClusterService
    {
        private readonly ICallerProvider _callerProvider;

        public ClusterService(ICallerProvider callerProvider)
        {
            _callerProvider = callerProvider;
        }
    }
}
