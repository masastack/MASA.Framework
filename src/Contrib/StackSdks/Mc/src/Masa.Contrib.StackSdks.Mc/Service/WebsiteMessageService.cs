namespace Masa.Contrib.StackSdks.Mc.Service;

public class WebsiteMessageService : IWebsiteMessageService
{
    readonly ICaller _caller;
    readonly string _party = "api/website-message";

    public WebsiteMessageService(ICaller caller)
    {
        _caller = caller;
    }

    public async Task CheckAsync()
    {
        var requestUri = $"{_party}/Check";
        await _caller.PostAsync(requestUri, null);
    }

    public async Task DeleteAsync(Guid id)
    {
        var requestUri = $"{_party}/{id}";
        await _caller.DeleteAsync(requestUri, null);
    }

    public async Task<WebsiteMessageModel?> GetAsync(Guid id)
    {
        var requestUri = $"{_party}/{id}";
        return await _caller.GetAsync<WebsiteMessageModel>(requestUri);
    }

    public async Task<List<WebsiteMessageChannelModel>> GetChannelListAsync()
    {
        var requestUri = $"{_party}/GetChannelList";
        return await _caller.GetAsync<List<WebsiteMessageChannelModel>>(requestUri)??new();
    }

    public async Task<PaginatedListModel<WebsiteMessageModel>> GetListAsync(GetWebsiteMessageModel options)
    {
        var requestUri = $"{_party}";
        return await _caller.GetAsync<GetWebsiteMessageModel, PaginatedListModel<WebsiteMessageModel>>(requestUri, options) ?? new();
    }

    public async Task<List<WebsiteMessageModel>> GetNoticeListAsync(GetNoticeListModel options)
    {
        var requestUri = $"{_party}/GetNoticeList";
        return await _caller.GetAsync<GetNoticeListModel, List<WebsiteMessageModel>>(requestUri, options) ?? new();
    }

    public async Task ReadAsync(ReadWebsiteMessageModel options)
    {
        var requestUri = $"{_party}/Read";
        await _caller.PostAsync(requestUri, options);
    }

    public async Task SetAllReadAsync(ReadAllWebsiteMessageModel options)
    {
        var requestUri = $"{_party}/SetAllRead";
        await _caller.PostAsync(requestUri, options);
    }

    public async Task SendCheckNotificationAsync()
    {
        var requestUri = $"{_party}/SendCheckNotification";
        await _caller.PostAsync(requestUri, null);
    }

    public async Task SendGetNotificationAsync(List<string> userIds)
    {
        var requestUri = $"{_party}/SendGetNotification";
        await _caller.PostAsync(requestUri, userIds);
    }
}
