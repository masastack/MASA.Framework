中 | [EN](README.md)

提供[国际化](https://developer.mozilla.org/zh-CN/docs/Mozilla/Add-ons/WebExtensions/Internationalization)能力

## Masa.Contrib.Globalization.I18n

用例：

``` powershell
Install-Package Masa.Contrib.Globalization.I18n
```

### 入门

1. 默认资源文件夹结构

``` structure
- Resources
  - I18n
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
        "DisplayName":"中文简体",
        "Icon": "{Replace-Your-Icon}"
    },
    {
        "Culture":"en-US",
        "DisplayName":"English (United States)",
        "Icon": "{Replace-Your-Icon}"
    }
]
```

2. 注册使用I18N, 修改`Program.cs`

``` C#
services.AddI18N();
```

3. 如何使用I18N

* 从DI获取`II18N` (**II18N**是接口，支持从DI获取)
* 使用`I18N` (**I18N**是静态类)

以`I18N`为例:

``` C#
var home = I18N.T("Home"); //获取键值Home对应语言的值，此方法调用将返回"首页";
var name = I18N.T("User.Name");//输出：名称（支持嵌套）
```

### I18N提供

* SetCulture (string cultureName): 将CurrentCulture切换成zh-CN，它更改后数字、日期等表示格式也随之改变
* SetUiCulture (string cultureName): 将界面语言(CurrentUICulture)切换成zh-CN
* GetCultureInfo (): 得到CurrentCulture配置
* GetUiCultureInfo (): 得到界面语言(CurrentUICulture)
* T (string name): 得到参数'name'在当前语言下的值
  * 当参数'name'在当前语言下未配置时返回参数'name'的值
* T (string name, bool returnKey): 得到参数'name'在当前语言下的值
  * returnKey: 当参数'name'在当前语言下未配置时，是否返回参数`name`的值
* string T(string name, params object[] arguments): 得到参数'name'在当前语言下的值, 并根据传入参数得到最终的结果
  * arguments: 参数值, 例如: '{0}' 必须大于18, 则应该使用 I18N.T("Age",18)
  * 如果参数存在数字、日期等格式，它将随着CurrentCulture的更改更改变
* string T(string name, bool returnKey, params object[] arguments): 得到参数'name'在当前语言下的值, 并根据传入参数得到最终的结果
* GetLanguages(): 获取当前系统支持的所有语言