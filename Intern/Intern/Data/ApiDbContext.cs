using Intern.DataModels;
using Microsoft.EntityFrameworkCore;

namespace Intern.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
            // Ensure DB and tables are created if they don't exist
            Database.EnsureCreated();
        }

        public DbSet<UserDM> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optional: Fluent API configurations
        }
    }
}
