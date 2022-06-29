namespace Masa.Contrib.BasicAbility.Mc.Service;

public class WebsiteMessageService : IWebsiteMessageService
{
    readonly ICallerProvider _callerProvider;
    readonly string _party = "api/website-message";

    public WebsiteMessageService(ICallerProvider callerProvider)
    {
        _callerProvider = callerProvider;
    }

    public async Task CheckAsync()
    {
        var requestUri = $"{_party}/Check";
        await _callerProvider.PostAsync(requestUri, null);
    }

    public async Task DeleteAsync(Guid id)
    {
        var requestUri = $"{_party}/{id}";
        await _callerProvider.DeleteAsync(requestUri, null);
    }

    public async Task<WebsiteMessageModel?> GetAsync(Guid id)
    {
        var requestUri = $"{_party}/{id}";
        return await _callerProvider.GetAsync<WebsiteMessageModel>(requestUri);
    }

    public async Task<List<WebsiteMessageChannelModel>> GetChannelListAsync()
    {
        var requestUri = $"{_party}/GetChannelList";
        return await _callerProvider.GetAsync<List<WebsiteMessageChannelModel>>(requestUri)??new();
    }

    public async Task<PaginatedListModel<WebsiteMessageModel>> GetListAsync(GetWebsiteMessageModel options)
    {
        var requestUri = $"{_party}";
        return await _callerProvider.GetAsync<GetWebsiteMessageModel, PaginatedListModel<WebsiteMessageModel>>(requestUri, options) ?? new();
    }

    public async Task<List<WebsiteMessageModel>> GetNoticeListAsync(GetNoticeListModel options)
    {
        var requestUri = $"{_party}/GetNoticeList";
        return await _callerProvider.GetAsync<GetNoticeListModel, List<WebsiteMessageModel>>(requestUri, options) ?? new();
    }

    public async Task ReadAsync(ReadWebsiteMessageModel options)
    {
        var requestUri = $"{_party}/Read";
        await _callerProvider.PostAsync(requestUri, options);
    }

    public async Task SetAllReadAsync(ReadAllWebsiteMessageModel options)
    {
        var requestUri = $"{_party}/SetAllRead";
        await _callerProvider.PostAsync(requestUri, options);
    }
}
