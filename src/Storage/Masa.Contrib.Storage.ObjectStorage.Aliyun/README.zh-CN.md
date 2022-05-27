中 | [EN](README.md)

## Masa.Contrib.Storage.ObjectStorage.Aliyun

用例：

```C#
Install-Package Masa.Contrib.Storage.ObjectStorage.Aliyun
```

支持：

* GetSecurityToken: 获取安全令牌 (需提供Sts RegionId、RoleArn、RoleSessionName)
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
    "RegionId": "Replace-With-Your-RegionId",//https://help.aliyun.com/document_detail/371859.html
    "DurationSeconds": 3600,//选填、默认: 3600s
    "Storage": {
      "Endpoint": "Replace-With-Your-Endpoint",//https://help.aliyun.com/document_detail/31837.html
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
builder.Services.AddAliyunStorage(new AliyunStorageOptions(configuration["Aliyun:AccessKeyId"], configuration["Aliyun:AccessKeySecret"], configuration["Aliyun:RoleArn"], configuration["Aliyun:RoleSessionName"])
{
  Sts = new AliyunStsOptions(configuration["Aliyun:RegionId"]);
});
```

### 用法3:

1. 添加阿里云存储服务

```C#
var configuration = builder.Configuration;
builder.Services.AddAliyunStorage(() => new AliyunStorageOptions(configuration["Aliyun:AccessKeyId"], configuration["Aliyun:AccessKeySecret"], configuration["Aliyun:RoleArn"], configuration["Aliyun:RoleSessionName"])
{
  Sts = new AliyunStsOptions(configuration["Aliyun:RegionId"])
});
```

> 与用法2的区别在于延缓获取配置

