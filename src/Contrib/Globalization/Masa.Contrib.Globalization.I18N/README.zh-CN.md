中 | [EN](README.md)

提供[国际化](https://developer.mozilla.org/zh-CN/docs/Mozilla/Add-ons/WebExtensions/Internationalization)能力

## Masa.Contrib.Globalization.I18N

用例：

``` powershell
Install-Package Masa.Contrib.Globalization.I18N
```

### 入门

1. 默认资源文件夹结构

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
    "Home":"首页",
    "Docs":"文档",
    "Blog":"博客",
    "Team":"团队",
    "Search":"搜索",
    "User":{
        "Name":"名称"
    }
}
```

* supportedCultures.json

``` supportedCultures.json
[
    {
        "Culture":"zh-CN",
        "Display":"中文简体",
        "Icon": "{Replace-Your-Icon}"
    },
    {
        "Culture":"en-US",
        "Display":"English (United States)",
        "Icon": "{Replace-Your-Icon}"
    }
]
```

2. 注册使用I18N, 修改`Program.cs`

``` C#
services.AddI18N(options =>
{
    options.UseJson();//使用本地`Resources/i18n`文件夹下的i18n资源文件
});
```

3. 如何使用I18N

``` C#
II18N i18n;//从DI获取
i18n.SetCulture("zh-CN");//将语言切换成zh-CN
var home = i18n.T("Home");//获取键值Home对应语言的值，此方法调用将返回"首页";

var name = i18n.T("User.Name");//输出：名称（支持嵌套）
```

4. 获取所有语言

``` C#
ILanguageProvider languageProvider;//从DI获取
var languages = languageProvider.GetLanguages();
```