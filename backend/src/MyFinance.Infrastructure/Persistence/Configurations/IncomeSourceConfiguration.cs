using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinance.Domain.Entities;

namespace MyFinance.Infrastructure.Persistence.Configurations;

public sealed class IncomeSourceConfiguration : IEntityTypeConfiguration<IncomeSource>
{
    public void Configure(EntityTypeBuilder<IncomeSource> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();

        builder.Property(i => i.Description).IsRequired().HasMaxLength(150);
        builder.Property(i => i.DefaultAmount).HasColumnType("numeric(18,2)");
        builder.Property(i => i.RecurrenceType).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(i => i.StartDate).HasColumnType("date");
        builder.Property(i => i.EndDate).HasColumnType("date");
        builder.Property(i => i.Notes).HasMaxLength(500);
        builder.Property(i => i.CreatedAt).HasColumnType("timestamptz");
        builder.Property(i => i.UpdatedAt).HasColumnType("timestamptz");

        builder.HasIndex(i => i.UserId);
        builder.HasIndex(i => i.CategoryId);
        builder.HasOne<User>().WithMany().HasForeignKey(i => i.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Category>().WithMany().HasForeignKey(i => i.CategoryId).OnDelete(DeleteBehavior.Restrict);
    }
}