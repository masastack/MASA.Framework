中 | [EN](README.md)

## Masa.Contrib.Storage.ObjectStorage.Aliyun

用例：

```C#
Install-Package Masa.Contrib.Storage.ObjectStorage.Aliyun
```

支持：
* GetSecurityToken 获取安全令牌

### 用法1:

1. 配置appsettings.json
``` C#
{
  "AliyunOss": {
    "AccessKeyId": "Replace-With-Your-AccessKeyId",
    "AccessKeySecret": "Replace-With-Your-AccessKeySecret",
    "RegionId": "Replace-With-Your-RegionId",
    "RoleArn": "Replace-With-Your-RoleArn",
    "RoleSessionName": "Replace-With-Your-RoleSessionName",
    "DurationSeconds": 3600,//选填、默认: 3600s
    "Policy": "",//选填
    "TemporaryCredentialsCacheKey": "Aliyun.TemporaryCredentials"//选填、默认: Aliyun.TemporaryCredentials
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
builder.Services.AddAliyunStorage(new ALiYunStorageOptions(configuration["Aliyun:AccessKeyId"], configuration["Aliyun:AccessKeySecret"], configuration["Aliyun:RegionId"], configuration["Aliyun:RoleArn"], configuration["Aliyun:RoleSessionName"]));
```

### 用法3:

1. 添加阿里云存储服务

```C#
var configuration = builder.Configuration;
builder.Services.AddAliyunStorage(() => new ALiYunStorageOptions(configuration["Aliyun:AccessKeyId"], configuration["Aliyun:AccessKeySecret"], configuration["Aliyun:RegionId"], configuration["Aliyun:RoleArn"], configuration["Aliyun:RoleSessionName"]));
```

> 与用法2的区别在于配置更新后无需重启项目即可生效