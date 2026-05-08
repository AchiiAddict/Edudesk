using LearningService.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningService.Data
{
    public class LearningDbContext : DbContext
    {
        public LearningDbContext(DbContextOptions<LearningDbContext> options) : base(options) { }

        public DbSet<Course> Courses { get; set; }

        // ROW-LEVEL SECURITY*
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //test guid
            Guid currentTenantId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

            modelBuilder.Entity<Course>().HasQueryFilter(c => c.TenantId == currentTenantId);
        }
    }
}