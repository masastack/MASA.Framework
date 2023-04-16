// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.EFCore.Tests;

public class CustomDbContext : MasaDbContext, ICustomDbContext
{
    public virtual string Name => nameof(CustomDbContext);

    public CustomDbContext() { }

    public CustomDbContext(MasaDbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreatingExecuting(ModelBuilder modelBuilder)
    {
        modelBuilder.InitializeStudentConfiguration();
        modelBuilder.Entity<User>();
    }
}

public class CustomDbContext2 : CustomDbContext
{
    public override string Name => nameof(CustomDbContext2);

    public CustomDbContext2(MasaDbContextOptions options)
        : base(options)
    {
    }
}

public class CustomDbContext3 : CustomDbContext
{
    public override string Name => nameof(CustomDbContext3);

    public CustomDbContext3(MasaDbContextOptions<CustomDbContext3> options)
        : base(options)
    {
    }
}

public class CustomDbContext4 : MasaDbContext, ICustomDbContext
{
    public string Name => nameof(CustomDbContext4);

    protected override void OnModelCreatingExecuting(ModelBuilder modelBuilder)
    {
        modelBuilder.InitializeStudentConfiguration();
    }

    protected override void OnConfiguring(MasaDbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase($"{nameof(CustomDbContext4)}");
    }
}

public class CustomDbContext5 : MasaDbContext, ICustomDbContext
{
    public string Name => nameof(CustomDbContext5);

    private readonly IOptions<AppConfigOptions> _options;

    public CustomDbContext5(IOptions<AppConfigOptions> options)
    {
        _options = options;
    }

    protected override void OnModelCreatingExecuting(ModelBuilder modelBuilder)
    {
        modelBuilder.InitializeStudentConfiguration();
    }

    protected override void OnConfiguring(MasaDbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(_options.Value.DbConnectionString);
    }
}

public class CustomDbContext6 : MasaDbContext<CustomDbContext6>, ICustomDbContext
{
    public string Name => nameof(CustomDbContext6);

    public CustomDbContext6(MasaDbContextOptions<CustomDbContext6> options)
        : base(options)
    {
    }

    protected override void OnModelCreatingExecuting(ModelBuilder modelBuilder)
    {
        modelBuilder.InitializeStudentConfiguration();
    }
}

public class CustomDbContext7 : MasaDbContext<CustomDbContext7>, ICustomDbContext
{
    public string Name => nameof(CustomDbContext7);

    private readonly IOptions<AppConfigOptions> _options;

    public CustomDbContext7(IOptions<AppConfigOptions> options)
    {
        _options = options;
    }

    protected override void OnModelCreatingExecuting(ModelBuilder modelBuilder)
    {
        modelBuilder.InitializeStudentConfiguration();
    }

    protected override void OnConfiguring(MasaDbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(_options.Value.DbConnectionString);
    }
}


public class CustomDbContextByNotUseDatabase : MasaDbContext<CustomDbContextByNotUseDatabase>, ICustomDbContext
{
    public string Name => nameof(CustomDbContextByNotUseDatabase);

    protected override void OnModelCreatingExecuting(ModelBuilder modelBuilder)
    {
        modelBuilder.InitializeStudentConfiguration();
    }
}

public class CustomDbContextTrackingAll : CustomDbContext
{
    public override string Name => nameof(CustomDbContextTrackingAll);

    public CustomDbContextTrackingAll(MasaDbContextOptions<CustomDbContextTrackingAll> options) : base(options)
    {
        base.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
    }
}

public class CustomDbContextTrackingAllByOnConfiguring : CustomDbContext
{
    public override string Name => nameof(CustomDbContextTrackingAllByOnConfiguring);

    protected override void OnConfiguring(MasaDbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase($"test-{Guid.NewGuid()}");

        optionsBuilder.DbContextOptionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
    }
}
