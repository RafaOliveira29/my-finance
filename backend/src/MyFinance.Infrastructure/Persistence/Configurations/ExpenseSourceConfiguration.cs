using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinance.Domain.Entities;

namespace MyFinance.Infrastructure.Persistence.Configurations;

public sealed class ExpenseSourceConfiguration : IEntityTypeConfiguration<ExpenseSource>
{
    public void Configure(EntityTypeBuilder<ExpenseSource> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Description).IsRequired().HasMaxLength(150);
        builder.Property(e => e.ExpenseKind).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(e => e.DefaultAmount).HasColumnType("numeric(18,2)");
        builder.Property(e => e.RecurrenceType).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(e => e.StartDate).HasColumnType("date");
        builder.Property(e => e.EndDate).HasColumnType("date");
        builder.Property(e => e.Notes).HasMaxLength(500);
        builder.Property(e => e.CreatedAt).HasColumnType("timestamptz");
        builder.Property(e => e.UpdatedAt).HasColumnType("timestamptz");

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.CategoryId);
        builder.HasOne<User>().WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Category>().WithMany().HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict);
    }
}