// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Pm.Model;

public class EnvironmentClusterModel
{
    public int Id { get; set; }

    public string EnvironmentName { get; set; } = "";

    public string EnvironmentColor { get; set; } = "";

    public string ClusterName { get; set; } = "";

    public string EnvironmentClusterName
    {
        get
        {
            return $"{EnvironmentName}/{ClusterName}";
        }
    }
}
