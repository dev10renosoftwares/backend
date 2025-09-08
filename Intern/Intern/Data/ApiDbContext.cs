using Intern.DataModels.Exams;
using Intern.DataModels.User;
using Microsoft.EntityFrameworkCore;

namespace Intern.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
            // For dev/testing, EnsureCreated is ok. 
            // But in production, prefer Migrations instead of EnsureCreated.
            Database.EnsureCreated();
        }

        // -------------------------
        // User DbSets
        // -------------------------
        public DbSet<ApplicationUserDM> ApplicationUsers { get; set; }
        public DbSet<ClientUserDM> ClientUsers { get; set; }
        public DbSet<ExternalUserDM> ExternalUsers { get; set; }

        // -------------------------
        // Exam DbSets
        // -------------------------
        public DbSet<DepartmentDM> Departments { get; set; }
        public DbSet<PostDM> Posts { get; set; }
        public DbSet<SubjectDM> Subjects { get; set; }
        public DbSet<MCQsDM> MCQs { get; set; }

        // -------------------------
        // Mapping Tables
        // -------------------------
        public DbSet<DepartmentPostsDM> DepartmentPosts { get; set; }
        public DbSet<SubjectPostDM> SubjectPosts { get; set; }
        public DbSet<MCQPostSubjectDM> MCQPostSubjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------------------------
            // DepartmentPostsDM Config
            // -------------------------
            modelBuilder.Entity<DepartmentPostsDM>()
                .HasIndex(dp => new { dp.DepartmentId, dp.PostId })
                .IsUnique();

            modelBuilder.Entity<DepartmentPostsDM>()
                .HasOne(dp => dp.Department)
                .WithMany(d => d.DepartmentPosts)
                .HasForeignKey(dp => dp.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DepartmentPostsDM>()
                .HasOne(dp => dp.Post)
                .WithMany(p => p.DepartmentPosts)
                .HasForeignKey(dp => dp.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            // SubjectPostDM Config
            // -------------------------
            modelBuilder.Entity<SubjectPostDM>()
                .HasIndex(sp => new { sp.SubjectId, sp.PostId })
                .IsUnique();

            modelBuilder.Entity<SubjectPostDM>()
                .HasOne(sp => sp.Subject)
                .WithMany(s => s.SubjectPosts)
                .HasForeignKey(sp => sp.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubjectPostDM>()
                .HasOne(sp => sp.Post)
                .WithMany(p => p.SubjectPosts)
                .HasForeignKey(sp => sp.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            // MCQPostSubjectDM Config
            // -------------------------
            modelBuilder.Entity<MCQPostSubjectDM>()
                .HasIndex(mps => new { mps.MCQId, mps.SubjectPostId, mps.DepartmentPostId })
                .IsUnique();

            modelBuilder.Entity<MCQPostSubjectDM>()
                .HasOne(mps => mps.MCQ)
                .WithMany(m => m.MCQPostSubjects)
                .HasForeignKey(mps => mps.MCQId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MCQPostSubjectDM>()
                .HasOne(mps => mps.SubjectPost)
                .WithMany(sp => sp.MCQPostSubjects)
                .HasForeignKey(mps => mps.SubjectPostId)
                .OnDelete(DeleteBehavior.SetNull);  // allowed because FK is nullable

            modelBuilder.Entity<MCQPostSubjectDM>()
                .HasOne(mps => mps.DepartmentPosts)
                .WithMany(dp => dp.MCQPostSubjects)
                .HasForeignKey(mps => mps.DepartmentPostId)
                .OnDelete(DeleteBehavior.SetNull);  // allowed because FK is nullable
        }
    }
}
