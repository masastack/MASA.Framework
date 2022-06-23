中 | [EN](README.md)

## Masa.Contrib.Storage.ObjectStorage.Aliyun

用例：

```C#
Install-Package Masa.Contrib.Storage.ObjectStorage.Aliyun
```

支持：

* GetSecurityToken: 获取安全令牌 (Sts RegionId、RoleArn、RoleSessionName是必须的)
* GetObjectAsync: 获取对象数据的流
* PutObjectAsync: 通过Stream上传对象
* ObjectExistsAsync: 判断对象是否存在
* DeleteObjectAsync: 删除对象

### 用法1:

1. 配置appsettings.json

``` C#
{
  "Aliyun": {
    "AccessKeyId": "Replace-With-Your-AccessKeyId",
    "AccessKeySecret": "Replace-With-Your-AccessKeySecret",
    "Sts": :{
      "RegionId":"Replace-With-Your-Sts-RegionId",//https://help.aliyun.com/document_detail/371859.html
      "DurationSeconds":3600,//临时证书有效期, default: 3600秒
      "EarlyExpires":10//default: 10秒
    },
    "Storage": {
      "Endpoint": "Replace-With-Your-Endpoint",//https://help.aliyun.com/document_detail/31837.html
      "RoleArn": "Replace-With-Your-RoleArn",
      "RoleSessionName": "Replace-With-Your-RoleSessionName",
      "TemporaryCredentialsCacheKey": "Aliyun.Storage.TemporaryCredentials",//选填、默认: Aliyun.Storage.TemporaryCredentials
      "Policy": "",//选填
      "BucketNames" : {
        "DefaultBucketName" : ""
      }
    }
  }
}
```

2. 添加阿里云存储服务

```C#
builder.Services.AddAliyunStorage();
```

3. 从DI获取`IClient`

    ``` C#
    //上传文件
    var fileStream = File.OpenRead("D://favicon.png");//更换本地文件路径
    await serviceProvider.GetService<IClient>().PutObjectAsync("storage1-test", "1.png", fileStream);
    ```

### 用法2：

1. 添加阿里云存储服务

```C#
var configuration = builder.Configuration;
var aliyunStorageOptions = new AliyunStorageOptions(
    configuration["Aliyun:AccessKeyId"],
    configuration["Aliyun:AccessKeySecret"],
    configuration["Aliyun:Endpoint"],
    configuration["Aliyun:RoleArn"],
    configuration["Aliyun:RoleSessionName"])
{
    Sts = new AliyunStsOptions(configuration["Aliyun:RegionId"]);
};
builder.Services.AddAliyunStorage(aliyunStorageOptions);
```

2. 从DI获取`IClient`，并使用相应的方法

### 用法3:

1. 添加阿里云存储服务

```C#
var configuration = builder.Configuration;
builder.Services.AddAliyunStorage(() =>
{
    return new AliyunStorageOptions(
        configuration["Aliyun:AccessKeyId"],
        configuration["Aliyun:AccessKeySecret"],
        configuration["Aliyun:Endpoint"],
        configuration["Aliyun:RoleArn"],
        configuration["Aliyun:RoleSessionName"])
    {
        Sts = new AliyunStsOptions(configuration["Aliyun:RegionId"])
    };
});
```

2. 从DI获取`IClient`，并使用相应的方法

> 与用法2的区别在于延缓获取配置

### 用法4:

1. 添加阿里云存储服务

```C#
builder.Services.AddAliyunStorage((serviceProvider) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return new AliyunStorageOptions(
        configuration["Aliyun:AccessKeyId"],
        configuration["Aliyun:AccessKeySecret"],
        configuration["Aliyun:Endpoint"],
        configuration["Aliyun:RoleArn"],
        configuration["Aliyun:RoleSessionName"])
    {
        Sts = new AliyunStsOptions(configuration["Aliyun:RegionId"])
    };
});
```

2. 从DI获取`IClient`，并使用相应的方法

> 与用法3的区别在于可以通过serviceProvider获取配置所需要的服务，最后返回配置对象

> 如果不需要使用临时凭证，可不配置Sts、RoleArn、RoleSessionName参数