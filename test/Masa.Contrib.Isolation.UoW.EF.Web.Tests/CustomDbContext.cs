// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.UoW.EF.Web.Tests;

public class CustomDbContext : IsolationDbContext
{
    public CustomDbContext(MasaDbContextOptions options) : base(options) { }

    public DbSet<User> User { get; set; }

    public DbSet<Role> Role { get; set; }

    protected override void OnModelCreatingExecuting(ModelBuilder builder)
    {
        builder.Entity<User>(ConfigureUserEntry);
        builder.Entity<Role>(ConfigureRoleEntry);
    }

    void ConfigureUserEntry(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .IsRequired();

        builder.Property(user => user.Account)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(user => user.Account)
            .HasMaxLength(50)
            .IsRequired();
    }

    void ConfigureRoleEntry(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(role => role.Id);

        builder.Property(e => e.Id)
            .IsRequired();

        builder.Property(e => e.Name)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Quantity)
            .IsRequired();
    }
}

public class User : IIsolation<int>
{
    public Guid Id { get; private set; }

    public string Account { get; set; } = default!;

    public string Password { get; set; } = default!;

    public int TenantId { get; set; }

    public string Environment { get; set; }

    public User()
    {
        this.Id = Guid.NewGuid();
    }
}

public class Role : IIsolation<int>, ISoftDelete
{
    public Guid Id { get; private set; }

    public string Name { get; set; }

    public int Quantity { get; set; }

    public bool IsDeleted { get; set; }

    public int TenantId { get; set; }

    public string Environment { get; set; }

    public Role()
    {
        this.Id = Guid.NewGuid();
    }
}
