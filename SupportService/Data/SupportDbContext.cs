using Microsoft.EntityFrameworkCore;
using SupportService.Models;

namespace SupportService.Data
{
    public class SupportDbContext : DbContext
    {
        public SupportDbContext(DbContextOptions<SupportDbContext> options) : base(options) { }

        public DbSet<Ticket> Tickets { get; set; }

        // ROW-LEVEL SECURITY*
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           //test guid

            Guid currentTenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

            modelBuilder.Entity<Ticket>().HasQueryFilter(t => t.TenantId == currentTenantId);
        }
    }
}