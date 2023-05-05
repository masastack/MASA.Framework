// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.EFCore.Tests.EntityConfigurations;

public class StudentEntityTypeConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("masa_students");

        builder.Property(s => s.Id).IsRequired();

        builder.OwnsOne(s => s.Address);

        builder
            .HasMany(u => u.Hobbies)
            .WithOne(ur => ur.Student).HasForeignKey(ur => ur.StudentId);
    }
}
