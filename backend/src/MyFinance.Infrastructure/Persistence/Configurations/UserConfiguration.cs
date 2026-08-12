using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinance.Domain.Entities;

namespace MyFinance.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever(); // Id nasce no domínio (factory).

        builder.Property(u => u.Name).IsRequired().HasMaxLength(150);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.PasswordHash).IsRequired();

        builder.Property(u => u.CreatedAt).HasColumnType("timestamptz");
        builder.Property(u => u.UpdatedAt).HasColumnType("timestamptz");
    }
}
