[中](README.zh-CN.md) | EN

## Masa.Contrib.Storage.ObjectStorage.Aliyun

Example：

````C#
Install-Package Masa.Contrib.Storage.ObjectStorage.Aliyun
````

support:
* GetSecurityToken: Gets the security token(RoleArn, RoleSessionName are required)
* GetObjectAsync: Gets the stream of object data
* PutObjectAsync: Upload objects via Stream
* ObjectExistsAsync: Determine whether the object exists
* DeleteObjectAsync: Delete object

### Usage 1:

1. Configure appsettings.json
```` C#
{
  "Aliyun": {
    "AccessKeyId": "Replace-With-Your-AccessKeyId",
    "AccessKeySecret": "Replace-With-Your-AccessKeySecret",
    "Sts": :{
      "RegionId": "Replace-With-Your-RegionId",//https://www.alibabacloud.com/help/en/resource-access-management/latest/endpoints#reference-sdg-3pv-xdb
      "DurationSeconds": 3600,//Temporary certificate validity period, default: 3600s
      "EarlyExpires": 10//default: 10s
    },
    "Storage": {
      "Endpoint": "Replace-With-Your-Endpoint",//https://www.alibabacloud.com/help/en/object-storage-service/latest/regions-and-endpoints#section-plb-2vy-5db
      "RoleArn": "Replace-With-Your-RoleArn",
      "RoleSessionName": "Replace-With-Your-RoleSessionName",
      "TemporaryCredentialsCacheKey": "Aliyun.Storage.TemporaryCredentials",//optional, default: Aliyun.Storage.TemporaryCredentials
      "Policy": ""//optional
    }
  }
}
````

2. Add Alibaba Cloud Storage Service

````C#
builder.Services.AddAliyunStorage();
````

### Usage 2:

1. Add Alibaba Cloud Storage Service

````C#
var configuration = builder.Configuration;
builder.Services.AddAliyunStorage(new AliyunStorageOptions(configuration["Aliyun:AccessKeyId"], configuration["Aliyun:AccessKeySecret"], configuration["Aliyun:RoleArn"], configuration["Aliyun:RoleSessionName"])
{
  Sts = new AliyunStsOptions(configuration["Aliyun:RegionId"]);
});
````

### Usage 3:

1. Add Alibaba Cloud Storage Service

````C#
var configuration = builder.Configuration;
builder.Services.AddAliyunStorage(() => new AliyunStorageOptions(configuration["Aliyun:AccessKeyId"], configuration["Aliyun:AccessKeySecret"], configuration["Aliyun:RoleArn"], configuration["Aliyun:RoleSessionName"])
{
  Sts = new AliyunStsOptions(configuration["Aliyun:RegionId"])
});
````

> The difference from usage 2 is that the configuration can take effect without restarting the project after the configuration update