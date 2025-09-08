using Google;
using Intern.Common.Helpers;
using Intern.Data;
using Intern.DataModels.Exams;
using Intern.DataModels.User;
using Intern.ServiceModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Intern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly PasswordHelper _passwordHelper;
        public SeedController(ApiDbContext context, PasswordHelper passwordHelper)
        {
            _context = context;
            _passwordHelper = passwordHelper;
        }

        [HttpPost("Initialize")]
        public async Task<IActionResult> Initialize()
        {
            // ✅ Prevent running twice
            if (_context.ApplicationUsers.Any())
                return BadRequest("Database already seeded.");
            #region App Users
            var appUsers = new[]
            {
                new ApplicationUserDM
                { 
                    CreatedOnUtc = DateTime.UtcNow,
                    CreatedBy = "Seed User",
                    LoginId = "SuperAdmin1",
                    Email = "super1@gmail.com",
                    Password = _passwordHelper.HashPassword("pass123"),
                    Role = DataModels.Enums.UserRoleDM.SuperAdmin                  

                }
            };
            _context.ApplicationUsers.AddRange(appUsers);
            await _context.SaveChangesAsync();
            #endregion App Users

            #region ClientUsers
            var users = new[]
            {
                new ClientUserDM
                {
                    CreatedOnUtc = DateTime.UtcNow,
                    CreatedBy = "Seeder",
                    LoginId = "client1",
                    Email = "client1@gmail.com",
                    Password = _passwordHelper.HashPassword("pass123"),
                    Role = DataModels.Enums.UserRoleDM.ClientEmployee,
                    MobileNumber = "9906612121",
                    IsActive = true,
                    IsEmailConfirmed = true,
                    IsMobileNumberConfirmed = true

                },
                new ClientUserDM
                {
                    CreatedOnUtc = DateTime.UtcNow,
                    CreatedBy = "Seeder",
                    LoginId = "client2",
                    Email = "client2@gmail.com",
                    Password = _passwordHelper.HashPassword("pass123"),
                    Role = DataModels.Enums.UserRoleDM.ClientEmployee,
                    MobileNumber = "9906612122",
                    IsActive = true,
                    IsEmailConfirmed = true,
                    IsMobileNumberConfirmed = true

                }
            };
            _context.ClientUsers.AddRange(users);
            await _context.SaveChangesAsync();

            #endregion ClientUsers


            var departments = new[]
            {
                new DepartmentDM { DepartmentName = "Health", Description = "Health Department" },
                new DepartmentDM { DepartmentName = "Education", Description = "Education Department" },
                new DepartmentDM { DepartmentName = "IT", Description = "Information Technology" },
                new DepartmentDM { DepartmentName = "Revenue", Description = "Revenue Department" }
            };
            _context.Departments.AddRange(departments);
            await _context.SaveChangesAsync();

            // -----------------------------
            // Posts
            // -----------------------------
            var posts = new[]
            {
                new PostDM { PostName = "Junior Assistant", Description = "Clerical post in department", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Patwari", Description = "Land records officer", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Staff Nurse", Description = "Healthcare staff" , CreatedOnUtc = DateTime.UtcNow},
                new PostDM { PostName = "Pharmacist", Description = "Medicine dispenser",CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Computer Assistant", Description = "IT Support role", CreatedOnUtc = DateTime.UtcNow }
            };
            _context.Posts.AddRange(posts);
            await _context.SaveChangesAsync();

            var deptPosts = new[]
            {
                new DepartmentPostsDM
                {
                    DepartmentId = departments.First(d => d.DepartmentName == "Health").Id,
                    PostId = posts.First(p => p.PostName == "Staff Nurse").Id,
                    NotificationNumber = "H-001",
                    PostDate = DateTime.UtcNow
                },

                // Health → Pharmacist
                new DepartmentPostsDM
                {
                    DepartmentId = departments.First(d => d.DepartmentName == "Health").Id,
                    PostId = posts.First(p => p.PostName == "Pharmacist").Id,
                    NotificationNumber = "H-002",
                    PostDate = DateTime.UtcNow
                },
                
                // Education → Junior Assistant
                new DepartmentPostsDM
                {
                    DepartmentId = departments.First(d => d.DepartmentName == "Education").Id,
                    PostId = posts.First(p => p.PostName == "Junior Assistant").Id,
                    NotificationNumber = "E-001",
                    PostDate = DateTime.UtcNow
                },
                
                // Revenue → Patwari
                new DepartmentPostsDM
                {
                    DepartmentId = departments.First(d => d.DepartmentName == "Revenue").Id,
                    PostId = posts.First(p => p.PostName == "Patwari").Id,
                    NotificationNumber = "R-001",
                    PostDate = DateTime.UtcNow
                },
                
                // IT → Computer Assistant
                new DepartmentPostsDM
                {
                    DepartmentId = departments.First(d => d.DepartmentName == "IT").Id,
                    PostId = posts.First(p => p.PostName == "Computer Assistant").Id,
                    NotificationNumber = "IT-001",
                    PostDate = DateTime.UtcNow
                }
            };

            _context.DepartmentPosts.AddRange(deptPosts);
            await _context.SaveChangesAsync();
            // -----------------------------
            // Subjects
            // -----------------------------
            var subjects = new[]
            {
                new SubjectDM { SubjectName = "Mathematics", Description = "", CreatedBy = "Seeder", CreatedOnUtc = DateTime.UtcNow },
                new SubjectDM { SubjectName = "General Knowledge", Description = "", CreatedBy = "Seeder", CreatedOnUtc = DateTime.UtcNow },
                new SubjectDM {SubjectName = "Reasoning", Description = "", CreatedBy = "Seeder", CreatedOnUtc = DateTime.UtcNow},
                new SubjectDM {SubjectName = "English", Description = "", CreatedBy = "Seeder", CreatedOnUtc = DateTime.UtcNow}
            };
            _context.Subjects.AddRange(subjects);
            await _context.SaveChangesAsync();

            var postSubjects = new[]
            {
                new SubjectPostDM { SubjectId = 1, PostId = 1 },
                new SubjectPostDM { SubjectId = 1, PostId = 2 },
                new SubjectPostDM { SubjectId = 1, PostId = 3 },
                new SubjectPostDM { SubjectId = 2, PostId = 1 },
                new SubjectPostDM { SubjectId = 2, PostId = 2 },
                new SubjectPostDM { SubjectId = 2, PostId = 3 },
                new SubjectPostDM { SubjectId = 3, PostId = 1 },
                new SubjectPostDM { SubjectId = 3, PostId = 2 },
                new SubjectPostDM { SubjectId = 3, PostId = 3 },
                new SubjectPostDM { SubjectId = 4, PostId = 1 },
                new SubjectPostDM { SubjectId = 4, PostId = 2 },
                new SubjectPostDM { SubjectId = 4, PostId = 3 },
                new SubjectPostDM { SubjectId = 4, PostId = 4 },
                new SubjectPostDM { SubjectId = 4, PostId = 5 },
            };

            _context.SubjectPosts.AddRange(postSubjects);
            await _context.SaveChangesAsync();


            return Ok("Database seeded successfully!");
        }
    }
}
