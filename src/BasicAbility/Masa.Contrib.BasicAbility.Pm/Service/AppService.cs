namespace Masa.Contrib.BasicAbility.Pm.Service
{
    public class AppService : IAppService
    {
        private readonly ICallerProvider _callerProvider;

        public AppService(ICallerProvider callerProvider)
        {
            _callerProvider = callerProvider;
        }
    }
}
