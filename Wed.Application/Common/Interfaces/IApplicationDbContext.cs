using Microsoft.EntityFrameworkCore;
using Wed.Domain.Entities;

namespace Wed.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        DbSet<Domain.Entities.Facility> Facilities { get;}
        DbSet<ApplicationUser> ApplicationUsers { get;}
    }
}
