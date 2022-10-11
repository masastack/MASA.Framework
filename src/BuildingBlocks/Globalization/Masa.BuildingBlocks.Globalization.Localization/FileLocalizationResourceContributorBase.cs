// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.Localization;

public abstract class FileLocalizationResourceContributorBase : LocalizationResourceContributorBase
{
    private readonly string _filePath;
    private readonly ILogger<FileLocalizationResourceContributorBase>? _logger;

    public FileLocalizationResourceContributorBase(Type resourceType,
        string cultureName,
        string filePath,
        ILoggerFactory? loggerFactory) : base(resourceType, cultureName)
    {
        _filePath = filePath;
        _logger = loggerFactory?.CreateLogger<FileLocalizationResourceContributorBase>();
    }

    protected override Dictionary<string, LocalizedString> GetDictionaries()
    {
        var fileContent = File.ReadAllText(_filePath);
        if (string.IsNullOrWhiteSpace(fileContent))
        {
            _logger?.LogWarning("File content is empty, the file is {file}", _filePath);
            return new Dictionary<string, LocalizedString>();
        }

        return ParseResourceFromFileContent(fileContent);
    }

    protected abstract Dictionary<string, LocalizedString> ParseResourceFromFileContent(string fileContent);
}
