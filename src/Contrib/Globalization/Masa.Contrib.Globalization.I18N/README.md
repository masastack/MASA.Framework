[中](README.zh-CN.md) | EN

Provide [Internationalization](https://developer.mozilla.org/zh-CN/docs/Mozilla/Add-ons/WebExtensions/Internationalization) capability

## Masa.Contrib.Globalization.I18N

Example:

``` powershell
Install-Package Masa.Contrib.Globalization.I18N
```

### getting Started

1. Default resource folder structure

``` structure
- Resources
  - i18n
    - en-US.json
    - zh-CN.json
    - supportedCultures.json
```

* en-US.json

``` en-US.json
{
    "Home":"Home",
    "Docs":"Docs",
    "Blog":"Blog",
    "Team":"Team",
    "Search":"Search"
    "User":{
        "Name":"Name"
    }
}
```

* zh-CN.json

``` zh-CN.json
{
    "Home":"Home",
    "Docs":"Documents",
    "Blog":"Blog",
    "Team":"Team",
    "Search":"Search",
    "User":{
        "Name":"Name"
    }
}
```

* supportedCultures.json

``` supportedCultures.json
[
    {
        "Culture":"zh-CN",
        "Display":"Simplified Chinese",
        "Icon": "{Replace-Your-Icon}"
    },
    {
        "Culture":"en-US",
        "Display":"English (United States)",
        "Icon": "{Replace-Your-Icon}"
    }
]
```

2. Register to use I18N, modify `Program.cs`

``` C#
services.AddI18N(options =>
{
    options.UseJson();//Use the i18n resource file in the local `Resources/i18n` folder
});
```

3. How to use I18N

``` C#
II18N i18n;//Get from DI
i18n.SetCulture("zh-CN");//Switch the language to zh-CN
var home = i18n.T("Home");//Get the value of the language corresponding to the key value Home, this method call will return "Home";

var name = i18n.T("User.Name");//Output: name (support nesting)
```

4. Get all languages

``` C#
ILanguageProvider languageProvider;//Get from DI
var languages = languageProvider.GetLanguages();
```