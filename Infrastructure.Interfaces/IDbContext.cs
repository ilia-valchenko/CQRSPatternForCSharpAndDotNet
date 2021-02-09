using System.Threading;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Interfaces
{
    public interface IDbContext
    {
        DbSet<Order> Orders { get; }

        Task<int> SaveChangesAsync(CancellationToken token = default);
    }
}