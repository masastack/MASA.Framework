namespace Masa.Contrib.BasicAbility.Auth.Service;

public class SubjectService : ISubjectService
{
    readonly ICallerProvider _callerProvider;

    public SubjectService(ICallerProvider callerProvider)
    {
        _callerProvider = callerProvider;
    }

    public async Task<List<SubjectModel>> GetListAsync(string filter)
    {
        var requestUri = $"api/subject/list";
        return await _callerProvider.GetAsync<object, List<SubjectModel>>(requestUri, new { filter }) ?? new();
    }
}

