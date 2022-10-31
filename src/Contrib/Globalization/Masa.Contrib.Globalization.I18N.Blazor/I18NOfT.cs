// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18N.Blazor;

[ExcludeFromCodeCoverage]
public class I18N<TResourceSource> : Masa.BuildingBlocks.Globalization.I18N.I18N<TResourceSource>
{
    private const string CULTURE_COOKIE_KEY = "Masa_I18nConfig_Culture";

    private const string GET_COOKIE_JS =
        "(function(name){const reg = new RegExp(`(^| )${name}=([^;]*)(;|$)`);const arr = document.cookie.match(reg);if (arr) {return unescape(arr[2]);}return null;})";

    private const string SET_COOKIE_JS =
        "(function(name,value){ var Days = 30;var exp = new Date();exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);document.cookie = `${name}=${escape(value.toString())};path=/;expires=${exp.toUTCString()}`;})";

    private readonly IJSRuntime _jsRuntime;

    public I18N(IJSRuntime jsRuntime) => _jsRuntime = jsRuntime;

    public override CultureInfo GetCultureInfo()
    {
        if (_jsRuntime is IJSInProcessRuntime jsInProcess)
        {
            var cultureName = jsInProcess.Invoke<string>("eval", $"{GET_COOKIE_JS}('{CULTURE_COOKIE_KEY}')");
            return new CultureInfo(cultureName);
        }
        return CultureInfo.CurrentUICulture;
    }

    public override void SetCulture(CultureInfo culture)
    {
        try
        {
            _jsRuntime.InvokeVoidAsync("eval", $"{SET_COOKIE_JS}('{CULTURE_COOKIE_KEY}','{culture.Name}')")
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
        catch (Exception ex)
        {
        }
    }
}
