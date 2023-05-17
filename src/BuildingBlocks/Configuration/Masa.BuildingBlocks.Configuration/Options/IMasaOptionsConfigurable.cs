// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration.Options;

/// <summary>
/// Automatic mapping relationship specification.
/// When ParentSection is Null or an empty string, the configuration will be mounted to the root node.
/// When Section is Null, the configuration will be mounted under the ParentSection node, and its node name is class name.
/// If Section is an empty string, it will be directly mounted under the ParentSection node
///
/// Implementations that inherit this interface will be automatically registered and complete the options pattern
/// </summary>
public interface IMasaOptionsConfigurable
{
    /// <summary>
    /// The name of the parent section, if it is empty, it will be mounted under SectionType, otherwise it will be mounted to the specified section under SectionType
    /// </summary>
    string? GetParentSection();

    /// <summary>
    /// The section null means same as the class name, else load from the specify section
    /// </summary>
    string? GetSection();

    SectionTypes GetSectionType();

    /// <summary>
    /// The name of the options instance
    /// Normally it is string.Empty
    /// </summary>
    string GetOptionsName();
}
