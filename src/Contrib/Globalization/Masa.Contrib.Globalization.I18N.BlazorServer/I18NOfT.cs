// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18N.BlazorServer;

[ExcludeFromCodeCoverage]
public class I18NOfT<TResourceSource> : BuildingBlocks.Globalization.I18N.I18NOfT<TResourceSource>
{
    private const string CULTURE_COOKIE_KEY = ".AspNetCore.Culture";

    private const string SET_COOKIE_JS =
        "(function(name,value){ var Days = 30;var exp = new Date();exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);document.cookie = `${name}=${escape(value.toString())};path=/;expires=${exp.toUTCString()}`;})";

    private readonly IJSRuntime _jsRuntime;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<CultureSettings> _options;

    public I18NOfT(IJSRuntime jsRuntime, IHttpContextAccessor httpContextAccessor, IOptions<CultureSettings> options)
    {
        _jsRuntime = jsRuntime;
        _httpContextAccessor = httpContextAccessor;
        _options = options;
    }

    public override CultureInfo GetCultureInfo() => CultureInfo.CurrentCulture;

    public override CultureInfo GetUiCultureInfo() => CultureInfo.CurrentUICulture;

    public override void SetCulture(CultureInfo culture)
        => SetCultureCore(culture, GetUiCultureInfo());

    public override void SetUiCulture(CultureInfo culture) => SetCultureCore(GetCultureInfo(), culture);

    private RequestCulture GetSelectCulture()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return new RequestCulture(_options.Value.DefaultCulture);

        var requestCulture = httpContext.Features.Get<IRequestCultureFeature>();
        return requestCulture?.RequestCulture ?? new RequestCulture(_options.Value.DefaultCulture);
    }

    private void SetCultureCore(CultureInfo culture, CultureInfo uiCulture)
    {
        try
        {
            var val = ($"c={culture.Name}|uic={uiCulture.Name}");
            _jsRuntime.InvokeVoidAsync("eval", $"{SET_COOKIE_JS}('{CULTURE_COOKIE_KEY}','{val}')");
        }
        catch (Exception ex)
        {
        }
    }
}
