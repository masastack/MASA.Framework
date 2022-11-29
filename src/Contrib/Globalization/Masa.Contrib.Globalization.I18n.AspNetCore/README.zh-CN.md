中 | [EN](README.md)

提供解析获取Culture的能力，配合[I18n](../Masa.Contrib.Globalization.I18n/README.zh-CN.md)来使用，目前支持三种方式进行切换语言:

* URL 参数 方式： ?culture=en-US，此方式优先级最高，格式为：culture=区域码
* Cookies 方式：cookie 格式为 c=%LANGCODE%|uic=%LANGCODE%，其中 c 是 Culture，uic 是 UICulture, 例如:

``` cookie
c=en-UK|uic=en-US
```

* 客户端浏览器语言自动匹配：如果前面两种方式都没有设置，支持自动根据客户端浏览器语言进行匹配。

> 详细[可参考](https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/localization?view=aspnetcore-7.0#localization-middleware)

## Masa.Contrib.Globalization.I18n.AspNetCore

用例：

``` powershell
Install-Package Masa.Contrib.Globalization.I18n.AspNetCore
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

2. 注册使用I18n, 修改`Program.cs`

``` C#
services.AddI18n();
```

3. 使用`Masa.Contrib.Globalization.I18n.AspNetCore`提供解析Culture的能力

``` C#
app.UseI18n();
```

4. 如何使用I18n

* 从DI获取`II18n` (**II18n**是接口，支持从DI获取)
* 使用`I18n` (**I18n**是静态类)

以`I18n`为例:

``` C#
var home = I18n.T("Home"); //获取键值Home对应语言的值，此方法调用将返回"首页";
var name = I18n.T("User.Name");//输出：名称（支持嵌套）
```