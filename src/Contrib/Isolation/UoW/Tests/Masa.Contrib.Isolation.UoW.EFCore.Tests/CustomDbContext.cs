// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.UoW.EFCore.Tests;

public class CustomDbContext : MasaDbContext
{
    public CustomDbContext(MasaDbContextOptions options) : base(options) { }

    public DbSet<User> User { get; set; }

    public DbSet<Tag> Tag { get; set; }

    protected override void OnModelCreatingExecuting(ModelBuilder builder)
    {
        builder.Entity<User>(ConfigureUserEntry);
    }

    void ConfigureUserEntry(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .IsRequired();

        builder.Property(e => e.Name)
            .HasMaxLength(6)
            .IsRequired();
    }
}

public class User
{
    public Guid Id { get; private set; }

    public string Name { get; set; } = default!;

    public User()
    {
        this.Id = Guid.NewGuid();
    }
}

public class Tag : ISoftDelete
{
    public Guid Id { get; private set; }

    public string Name { get; set; } = default!;

    public bool IsDeleted { get; protected set; }

    public Tag()
    {
        this.Id = Guid.NewGuid();
    }
}
