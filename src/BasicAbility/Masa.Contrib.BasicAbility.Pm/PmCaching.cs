
namespace Masa.Contrib.BasicAbility.Pm
{
    public class PmCaching : IPmCaching
    {
        private readonly ICallerProvider _callerProvider;

        public PmCaching(ICallerProvider callerProvider)
        {
            _callerProvider = callerProvider;
        }

        public async Task<List<ProjectModel>> GetProjectListAsync(string envName)
        {
            var requestUri = $"third-party/api/v1/env/{envName}";
            var result = await _callerProvider.GetAsync<List<ProjectModel>>(requestUri);

            return result ?? new List<ProjectModel>();
        }
    }
}
