// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration;

public static class ConfigurationConstant
{
    public const DynamicallyAccessedMemberTypes DYNAMICALLY_ACCESSED_MEMBERS = DynamicallyAccessedMemberTypes.PublicParameterlessConstructor;

    public const string ENVIRONMENT_VARIABLE_NAME = "ASPNETCORE_ENVIRONMENT";

    public const string APPID_VARIABLE_NAME = "AppId";

    public const string CLUSTER_VARIABLE_NAME = "Cluster";

    public const string DEFAULT_ENVIRONMENT = "Production";

    public const string DEFAULT_CLUSTER = "Default";
}
