// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class DefaultIdGeneratorFactory : AbstractMasaFactory<IIdGenerator, IdGeneratorFactoryOptions, IdGeneratorRelationOptions>,
    IIdGeneratorFactory
{
    private IGuidGenerator? _guidGenerator;
    private ISequentialGuidGenerator? _sequentialGuidGenerator;
    private ISnowflakeGenerator? _snowflakeGenerator;

    public IGuidGenerator GuidGenerator => _guidGenerator ??=
        ServiceProvider.GetService<IGuidGenerator>() ?? throw new Exception($"Unsupported {nameof(GuidGenerator)}");

    public ISequentialGuidGenerator SequentialGuidGenerator => _sequentialGuidGenerator ??=
        ServiceProvider.GetService<ISequentialGuidGenerator>() ?? throw new Exception($"Unsupported {nameof(SequentialGuidGenerator)}");

    public ISnowflakeGenerator SnowflakeGenerator => _snowflakeGenerator ??=
        ServiceProvider.GetService<ISnowflakeGenerator>() ?? throw new Exception($"Unsupported {nameof(SnowflakeGenerator)}");


    protected override string DefaultServiceNotFoundMessage { get; } =
        "No default IdGenerator found, you may need service.AddSimpleGuidGenerator()";

    protected override string SpecifyServiceNotFoundMessage { get; } =
        "Please make sure you have used [{name}] IdGenerator, it was not found";

    public DefaultIdGeneratorFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    private IdGeneratorRelationOptions? GetDefaultOptions(List<IdGeneratorRelationOptions> optionsList)
    {
        return optionsList.SingleOrDefault(c => c.Name == Options.DefaultName) ??
            optionsList.FirstOrDefault();
    }

    public IIdGenerator<TOut> Create<TOut>() where TOut : notnull
    {
        var idGenerator = Create();
        return idGenerator as IIdGenerator<TOut> ?? throw new Exception($"Unsupported {nameof(IIdGenerator<TOut>)}");
    }

    public IIdGenerator<TOut> Create<TOut>(string name) where TOut : notnull
    {
        var idGenerator = Create(name);
        return idGenerator as IIdGenerator<TOut> ?? throw new Exception($"Unsupported {nameof(IIdGenerator<TOut>)}");
    }
}
