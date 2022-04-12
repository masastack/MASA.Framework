[中](README.zh-CN.md) | EN

## Masa.Contrib.Storage.ObjectStorage.Aliyun

Example：

````C#
Install-Package Masa.Contrib.Storage.ObjectStorage.Aliyun
````

support:
* GetSecurityToken to get the security token

### Usage 1:

1. Configure appsettings.json
```` C#
{
  "Aliyun": {
    "AccessKeyId": "",
    "AccessKeySecret": "",
    "RegionId": "",
    "RoleArn": "",
    "RoleSessionName": "",
    "DurationSeconds": 3600,//optional, default: 3600s
    "Policy": "",//optional
    "TemporaryCredentialsCacheKey": "Aliyun.TemporaryCredentials"//Optional, default: Aliyun.TemporaryCredentials
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
builder.Services.AddAliyunStorage(new ALiYunStorageOptions("AccessKeyId", "AccessKeySecret", "regionId", "roleArn", "roleSessionName"));
````

### Usage 3:

1. Add Alibaba Cloud Storage Service

````C#
builder.Services.AddAliyunStorage(() => new ALiYunStorageOptions(configuration["Aliyun:AccessKeyId"], configuration["Aliyun:AccessKeySecret"], configuration["Aliyun:RegionId"], configuration["Aliyun:RoleArn"], configuration ["Aliyun:RoleSessionName"]));
````

> The difference from usage 2 is that the configuration can take effect without restarting the project after the configuration update