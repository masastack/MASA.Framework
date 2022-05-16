中 | [EN](README.md)

## Masa.Contrib.Storage.ObjectStorage.Aliyun

用例：

```C#
Install-Package Masa.Contrib.Storage.ObjectStorage.Aliyun
```

支持：

* GetSecurityToken: 获取安全令牌 (需提供RoleArn、RoleSessionName)
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
    "RegionId": "Replace-With-Your-RegionId",
    "Endpoint": "Replace-With-Your-Endpoint",
    "DurationSeconds": 3600,//选填、默认: 3600s
    "Storage": {
      "RoleArn": "Replace-With-Your-RoleArn",
      "RoleSessionName": "Replace-With-Your-RoleSessionName",
      "TemporaryCredentialsCacheKey": "Aliyun.Storage.TemporaryCredentials",//选填、默认: Aliyun.Storage.TemporaryCredentials
      "Policy": ""//选填
    }
  }
}
```

2. 添加阿里云存储服务

```C#
builder.Services.AddAliyunStorage();
```

### 用法2：

1. 添加阿里云存储服务

```C#
var configuration = builder.Configuration;
builder.Services.AddAliyunStorage(new AliyunStorageOptions(configuration["Aliyun:AccessKeyId"], configuration["Aliyun:AccessKeySecret"], configuration["Aliyun:RegionId"], configuration["Aliyun:RoleArn"], configuration["Aliyun:RoleSessionName"]));
```

### 用法3:

1. 添加阿里云存储服务

```C#
var configuration = builder.Configuration;
builder.Services.AddAliyunStorage(() => new AliyunStorageOptions(configuration["Aliyun:AccessKeyId"], configuration["Aliyun:AccessKeySecret"], configuration["Aliyun:RegionId"], configuration["Aliyun:RoleArn"], configuration["Aliyun:RoleSessionName"]));
```

> 与用法2的区别在于配置更新后无需重启项目即可生效

### 用法4:

1. 添加阿里云存储服务

```C#
var configuration = builder.Configuration;
builder.Services.AddAliyunStorage("AccessKeyId", "AccessKeySecret", HANG_ZHOUE_REGIONID, Options.Enum.EndpointMode.Public, options =>
{
    options.CallbackUrl = "Replace-With-Your-CallbackUrl";
});
```

