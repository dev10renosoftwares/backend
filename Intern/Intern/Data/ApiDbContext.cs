using Intern.DataModels.User;
using Microsoft.EntityFrameworkCore;

namespace Intern.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();   
        }

        // DbSets
        public DbSet<ApplicationUserDM> ApplicationUsers { get; set; }
        public DbSet<ClientUserDM> ClientUsers { get; set; }
        public DbSet<ExternalUserDM> ExternalUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUserDM>()
             .Property(u => u.Role)
             .HasConversion<string>();

            modelBuilder.Entity<ClientUserDM>()
                .Property(u => u.Role)
                .HasConversion<string>();

            // Relationship: ClientUser -> ExternalUser (1-to-many) with cascade delete
            modelBuilder.Entity<ExternalUserDM>()
                .HasOne(e => e.User)
                .WithMany(c => c.ExternalUsers)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: prevent a user from having duplicate login types
            modelBuilder.Entity<ExternalUserDM>()
                .HasIndex(e => new { e.UserId, e.LoginType })
                .IsUnique();*/
        }
    }
}
