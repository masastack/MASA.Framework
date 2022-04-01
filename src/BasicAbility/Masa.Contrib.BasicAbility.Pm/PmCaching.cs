
namespace Masa.Contrib.BasicAbility.Pm
{
    public class PmCaching : IPmCaching
    {
        private readonly IMemoryCacheClient _memoryCacheClient;
        private readonly ICallerProvider _callerProvider;

        public PmCaching(IMemoryCacheClient memoryCacheClient, ICallerProvider callerProvider)
        {
            _memoryCacheClient = memoryCacheClient;
            _callerProvider = callerProvider;
        }

        public async Task<List<Project>> GetProjectListAsync(string envName)
        {
            var keys = await _memoryCacheClient.GetAsync<List<string>>($"{PROJECT_KEY_PREFIX}.{envName.ToLower()}");
            if (keys == null)
            {
                return new List<Project>();
            }
            else
            {
                var projects = await _memoryCacheClient.GetListAsync<Project>(keys.ToArray());
                var result = projects.Where(project => project != null).ToList();

                return result!;
            }
        }

        public async Task<List<App>> GetAppListAsync(string envName)
        {
            var keys = await _memoryCacheClient.GetAsync<List<string>>($"{APP_KEY_PREFIX}.{envName.ToLower()}");
            if (keys == null)
            {
                return new List<App>();
            }
            else
            {
                var apps = await _memoryCacheClient.GetListAsync<App>(keys.ToArray());
                var result = apps.Where(app => app != null).ToList();

                return result!;
            }
        }

        public async Task<List<ProjectApps>> GetProjectAppsListAsync(string envName)
        {
            var projects = await GetProjectListAsync(envName);
            var apps = await GetAppListAsync(envName);

            List<ProjectApps> result = projects.Select(project => new ProjectApps
            {
                Id = project.Id,
                Name = project.Name,
                Identity = project.Identity
            }).ToList();

            result.ForEach(projectApp =>
            {
                apps.ForEach(app =>
                {
                    if (projectApp.Id == app.ProjectId)
                    {
                        projectApp.Apps.Add(app);
                    }
                });
            });

            return result;
        }
    }
}
