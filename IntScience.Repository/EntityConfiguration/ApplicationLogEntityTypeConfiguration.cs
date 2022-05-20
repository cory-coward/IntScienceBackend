using IntScience.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IntScience.Repository.EntityConfiguration;

public class ApplicationLogEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationLog>
{
    public void Configure(EntityTypeBuilder<ApplicationLog> builder)
    {
        builder.Property(a => a.Severity)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(a => a.Message)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(a => a.Created)
            .HasColumnType("date")
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("getdate()");
    }
}
