using IntScience.Repository.EntityConfiguration;
using IntScience.Repository.IdentityModels;
using IntScience.Repository.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IntScience.Repository;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<ApplicationLog> ApplicationLogs { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationLogEntityTypeConfiguration).Assembly);
    }
}
