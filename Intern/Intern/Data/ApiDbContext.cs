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
        public DbSet<NotificationsDM> Notifications { get; set; }

        public DbSet<UserTestDetailsDM> UserTestDetails { get; set; }
        public DbSet<SyllabusDM> Syllabus { get; set; }
        public DbSet<PostSyllabusDM> PostSyllabus { get; set; }
        public DbSet<PapersDM> PreviousYearPapers { get; set; }
        public DbSet<PostPapersDM> PostPreviousYearPapers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------------------------
            // DepartmentPostsDM Config
            // -------------------------
            modelBuilder.Entity<DepartmentPostsDM>()
                .HasIndex(dp => new { dp.DepartmentId, dp.PostId, dp.PostDate, dp.NotificationNumber })
                .IsUnique();

            modelBuilder.Entity<DepartmentPostsDM>()
                .HasOne(dp => dp.Department)
                .WithMany(d => d.DepartmentPosts)
                .HasForeignKey(dp => dp.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade); // Department → DepartmentPosts

            modelBuilder.Entity<DepartmentPostsDM>()
                .HasOne(dp => dp.Post)
                .WithMany(p => p.DepartmentPosts)
                .HasForeignKey(dp => dp.PostId)
                .OnDelete(DeleteBehavior.Cascade); // Post → DepartmentPosts

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
                .OnDelete(DeleteBehavior.Cascade); // Subject → SubjectPosts

            modelBuilder.Entity<SubjectPostDM>()
                .HasOne(sp => sp.Post)
                .WithMany(p => p.SubjectPosts)
                .HasForeignKey(sp => sp.PostId)
                .OnDelete(DeleteBehavior.Cascade); // Post → SubjectPosts

            // -------------------------
            // MCQPostSubjectDM Config
            // -------------------------
            modelBuilder.Entity<MCQPostSubjectDM>()
                .HasIndex(mps => new { mps.MCQId, mps.SubjectId, mps.PostId })
                .IsUnique();

            modelBuilder.Entity<MCQPostSubjectDM>()
                .HasOne(mps => mps.MCQ)
                .WithMany(m => m.MCQPostSubjects)
                .HasForeignKey(mps => mps.MCQId)
                .OnDelete(DeleteBehavior.Cascade); // MCQ → MCQPostSubjectDM

            modelBuilder.Entity<MCQPostSubjectDM>()
                .HasOne(mps => mps.Subject)
                .WithMany(s => s.MCQPostSubjects)
                .HasForeignKey(mps => mps.SubjectId)
                .OnDelete(DeleteBehavior.Cascade); // Subject → MCQPostSubjectDM

            modelBuilder.Entity<MCQPostSubjectDM>()
                .HasOne(mps => mps.Post)
                .WithMany(p => p.MCQPostSubjects)
                .HasForeignKey(mps => mps.PostId)
                .OnDelete(DeleteBehavior.Cascade); // Post → MCQPostSubjectDM


            // -------------------------
            // UserTestDetailsDM Config
            // -------------------------
            modelBuilder.Entity<UserTestDetailsDM>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTestDetails)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserTestDetailsDM>()
                .HasOne(ut => ut.Post)
                .WithMany(p => p.UserTestDetails)
                .HasForeignKey(ut => ut.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserTestDetailsDM>()
                .HasOne(ut => ut.Subject)
                .WithMany(s => s.UserTestDetails)
                .HasForeignKey(ut => ut.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
