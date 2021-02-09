using Domain;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.MsSql
{
    public class AppDbContext : DbContext, IDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasData(
                new Order { Id = 1, UserEmail = "test@test.test" },
                new Order { Id = 2, UserEmail = "other" });
        }
    }
}