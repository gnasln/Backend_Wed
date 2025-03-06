using Wed.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Wed.Domain.Entities;

namespace Wed.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
    public DbSet<Facility> Facilities => Set<Facility>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.UserName).IsUnique();
            entity.HasIndex(e => e.CellPhone).IsUnique();
        });

        modelBuilder.Entity<Facility>(entity =>
        {
            entity.Property(e => e.Name).IsRequired()
                    .HasMaxLength(255);
            entity.Property(e => e.Address).IsRequired()
                    .HasMaxLength(500);
            entity.Property(e => e.FacilityType).IsRequired()
                    .HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired()
                    .HasMaxLength(20);

        });
    }

}