using IntScience.Repository.IdentityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IntScience.Repository.EntityConfiguration;

public class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName)
            .HasMaxLength(50);
        builder.Property(u => u.LastName)
            .HasMaxLength(50);
        builder.Property(u => u.FullName)
            .HasMaxLength(100)
            .HasComputedColumnSql("[FirstName] + ' ' + [LastName]");
    }
}
