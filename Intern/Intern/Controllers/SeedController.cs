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
                    Name = "SuperAdmin",
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
                    Name = "clientEmployee1",
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
                    Name = "clientEmployee2",
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


            // -----------------------------
            // Departments
            // -----------------------------
            var departments = new[]
            {
                new DepartmentDM { DepartmentName = "Revenue", Description = "Revenue Department" },
                new DepartmentDM { DepartmentName = "Health & Medical", Description = "Health & Medical Department" },
                new DepartmentDM { DepartmentName = "Law and Justice", Description = "Law and Justice Department" },
                new DepartmentDM { DepartmentName = "Agriculture and Skill Development", Description = "Agriculture and Skill Development Department" },
                new DepartmentDM { DepartmentName = "Education Development", Description = "Education Development Department" },
                new DepartmentDM { DepartmentName = "Power Development", Description = "Power Development Department" },
                new DepartmentDM { DepartmentName = "Public Works (PWD)", Description = "Public Works Department" },
                new DepartmentDM { DepartmentName = "Social Welfare", Description = "Social Welfare Department" },
                new DepartmentDM { DepartmentName = "Finance and Accounts", Description = "Finance and Accounts Department" },
                new DepartmentDM { DepartmentName = "Forest and Environment", Description = "Forest and Environment Department" },
                new DepartmentDM { DepartmentName = "IT", Description = "Information Technology Department" },
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
                new PostDM { PostName = "Staff Nurse", Description = "Healthcare staff", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Pharmacist", Description = "Medicine dispenser", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Computer Assistant", Description = "IT Support role", CreatedOnUtc = DateTime.UtcNow },
                
                // Revenue
                new PostDM { PostName = "Naib Tehsildar", Description = "Revenue field officer", CreatedOnUtc = DateTime.UtcNow },
                
                // Health
                new PostDM { PostName = "Junior Pharmacist", Description = "Junior pharmacist role", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Lab Technician", Description = "Lab testing staff", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "MPHW", Description = "Multi Purpose Health Worker", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Nursing Aid", Description = "Nursing assistant", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "ECG Technician", Description = "Electrocardiogram technician", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Junior Lab Technician", Description = "Lab assistant", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Junior Health Inspector", Description = "Health inspector support", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Junior Dental Technician", Description = "Dental support technician", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Junior Store Clerk", Description = "Store handling clerk", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "BCG Technician", Description = "BCG vaccine technician", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Dresser", Description = "Medical dresser role", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "CSSD Attendant", Description = "Central sterile services department", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Assistant Scientific Officer", Description = "Scientific support staff", CreatedOnUtc = DateTime.UtcNow },
                
                // Law and Justice
                new PostDM { PostName = "Assistant Law Officer", Description = "Law officer", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Legal Assistant", Description = "Legal assistant", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Junior Legal Assistant", Description = "Junior legal staff", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Reader", Description = "Court reader", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Urdu Typist", Description = "Urdu language typist", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Judgement Writer", Description = "Court judgement writer", CreatedOnUtc = DateTime.UtcNow },
                
                // Agriculture
                new PostDM { PostName = "Agriculture Extension Assistant", Description = "Agriculture assistant", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Field Assistant", Description = "Field support staff", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Horticulture Assistant", Description = "Horticulture support", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Sericulture Assistant", Description = "Silk industry support", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Veterinary Assistant", Description = "Animal health assistant", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Soil Assistant", Description = "Soil testing staff", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Soil Conservation Assistant", Description = "Soil conservation support", CreatedOnUtc = DateTime.UtcNow },
                
                // Education
                new PostDM { PostName = "Junior Librarian", Description = "Library staff", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Studio Assistant", Description = "Education studio support", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Kitabat Instructor", Description = "Calligraphy instructor", CreatedOnUtc = DateTime.UtcNow },
                
                // Power Development
                new PostDM { PostName = "Junior Engineer (Electrical)", Description = "Electrical engineering role", CreatedOnUtc = DateTime.UtcNow },
                
                // Public Works
                new PostDM { PostName = "Junior Engineer (Civil)", Description = "Civil engineering role", CreatedOnUtc = DateTime.UtcNow },
                
                // Social Welfare
                new PostDM { PostName = "Supervisor", Description = "Supervision role", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Social Worker", Description = "Welfare support worker", CreatedOnUtc = DateTime.UtcNow },
                
                // Finance
                new PostDM { PostName = "Accounts Assistant", Description = "Accounts staff", CreatedOnUtc = DateTime.UtcNow },
                
                // Forest
                new PostDM { PostName = "Forester", Description = "Forest officer", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Wildlife Guard", Description = "Wildlife protection", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Forest Guard", Description = "Forest guard role", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Draftsman", Description = "Technical draftsman", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Assistant Information Officer", Description = "Information services support", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Lab Assistant", Description = "Lab assistant", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Data Operator", Description = "Data entry role", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Scientist A", Description = "Research scientist", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Research Assistant", Description = "Research support staff", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Junior Scientific Assistant", Description = "Junior science support", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Junior Environment Engineer", Description = "Environment engineering support", CreatedOnUtc = DateTime.UtcNow },
                new PostDM { PostName = "Field Inspector", Description = "Field inspection officer", CreatedOnUtc = DateTime.UtcNow }
            };

            _context.Posts.AddRange(posts);
            await _context.SaveChangesAsync();

            // -----------------------------
            // DepartmentPosts
            // -----------------------------
            var deptPosts = new[]
            {
                new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Revenue").Id, PostId = posts.First(p => p.PostName == "Junior Assistant").Id, NotificationNumber = "REV-001", PostDate = DateTime.UtcNow },
                new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Revenue").Id, PostId = posts.First(p => p.PostName == "Patwari").Id, NotificationNumber = "REV-002", PostDate = DateTime.UtcNow },
                new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Revenue").Id, PostId = posts.First(p => p.PostName == "Naib Tehsildar").Id, NotificationNumber = "REV-003",PostDate = DateTime.UtcNow},

               // ----------------- Health & Medical -----------------
               new DepartmentPostsDM{DepartmentId = departments.First(d => d.DepartmentName == "Health & Medical").Id,PostId = posts.First(p => p.PostName == "Staff Nurse").Id,NotificationNumber = "HLT-001",PostDate = DateTime.UtcNow},
               new DepartmentPostsDM{DepartmentId = departments.First(d => d.DepartmentName == "Health & Medical").Id,PostId = posts.First(p => p.PostName == "Pharmacist").Id,NotificationNumber = "HLT-002",PostDate = DateTime.UtcNow},
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Health & Medical").Id, PostId = posts.First(p => p.PostName == "Junior Pharmacist").Id, NotificationNumber = "HLT-003", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Health & Medical").Id, PostId = posts.First(p => p.PostName == "Lab Technician").Id, NotificationNumber = "HLT-004", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Health & Medical").Id, PostId = posts.First(p => p.PostName == "MPHW").Id, NotificationNumber = "HLT-005", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Health & Medical").Id, PostId = posts.First(p => p.PostName == "Nursing Aid").Id, NotificationNumber = "HLT-006", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Health & Medical").Id, PostId = posts.First(p => p.PostName == "ECG Technician").Id, NotificationNumber = "HLT-007", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Health & Medical").Id, PostId = posts.First(p => p.PostName == "Junior Lab Technician").Id, NotificationNumber = "HLT-008", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Health & Medical").Id, PostId = posts.First(p => p.PostName == "Junior Health Inspector").Id, NotificationNumber = "HLT-009", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Health & Medical").Id, PostId = posts.First(p => p.PostName == "Junior Dental Technician").Id, NotificationNumber = "HLT-010", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Health & Medical").Id, PostId = posts.First(p => p.PostName == "Junior Store Clerk").Id, NotificationNumber = "HLT-011", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Health & Medical").Id, PostId = posts.First(p => p.PostName == "BCG Technician").Id, NotificationNumber = "HLT-012", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Health & Medical").Id, PostId = posts.First(p => p.PostName == "Dresser").Id, NotificationNumber = "HLT-013", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Health & Medical").Id, PostId = posts.First(p => p.PostName == "CSSD Attendant").Id, NotificationNumber = "HLT-014", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Health & Medical").Id, PostId = posts.First(p => p.PostName == "Assistant Scientific Officer").Id, NotificationNumber = "HLT-015", PostDate = DateTime.UtcNow },
               
               // ----------------- Law and Justice -----------------
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Law and Justice").Id, PostId = posts.First(p => p.PostName == "Assistant Law Officer").Id, NotificationNumber = "LAW-001", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Law and Justice").Id, PostId = posts.First(p => p.PostName == "Legal Assistant").Id, NotificationNumber = "LAW-002", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Law and Justice").Id, PostId = posts.First(p => p.PostName == "Junior Legal Assistant").Id, NotificationNumber = "LAW-003", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Law and Justice").Id, PostId = posts.First(p => p.PostName == "Reader").Id, NotificationNumber = "LAW-004", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Law and Justice").Id, PostId = posts.First(p => p.PostName == "Urdu Typist").Id, NotificationNumber = "LAW-005", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Law and Justice").Id, PostId = posts.First(p => p.PostName == "Judgement Writer").Id, NotificationNumber = "LAW-006", PostDate = DateTime.UtcNow },
               
               // ----------------- Agriculture and Skill Development -----------------
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Agriculture and Skill Development").Id, PostId = posts.First(p => p.PostName == "Agriculture Extension Assistant").Id, NotificationNumber = "AGR-001", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Agriculture and Skill Development").Id, PostId = posts.First(p => p.PostName == "Field Assistant").Id, NotificationNumber = "AGR-002", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Agriculture and Skill Development").Id, PostId = posts.First(p => p.PostName == "Horticulture Assistant").Id, NotificationNumber = "AGR-003", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Agriculture and Skill Development").Id, PostId = posts.First(p => p.PostName == "Sericulture Assistant").Id, NotificationNumber = "AGR-004", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Agriculture and Skill Development").Id, PostId = posts.First(p => p.PostName == "Veterinary Assistant").Id, NotificationNumber = "AGR-005", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Agriculture and Skill Development").Id, PostId = posts.First(p => p.PostName == "Soil Assistant").Id, NotificationNumber = "AGR-006", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Agriculture and Skill Development").Id, PostId = posts.First(p => p.PostName == "Soil Conservation Assistant").Id, NotificationNumber = "AGR-007", PostDate = DateTime.UtcNow },
               
               // ----------------- Education Development -----------------
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Education Development").Id, PostId = posts.First(p => p.PostName == "Junior Librarian").Id, NotificationNumber = "EDU-001", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Education Development").Id, PostId = posts.First(p => p.PostName == "Studio Assistant").Id, NotificationNumber = "EDU-002", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Education Development").Id, PostId = posts.First(p => p.PostName == "Kitabat Instructor").Id, NotificationNumber = "EDU-003", PostDate = DateTime.UtcNow },
               
               // ----------------- Power Development -----------------
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Power Development").Id, PostId = posts.First(p => p.PostName == "Junior Engineer (Electrical)").Id, NotificationNumber = "PWR-001", PostDate = DateTime.UtcNow },
               
               // ----------------- Public Works (PWD) -----------------
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Public Works (PWD)").Id, PostId = posts.First(p => p.PostName == "Junior Engineer (Civil)").Id, NotificationNumber = "PWD-001", PostDate = DateTime.UtcNow },
               
               // ----------------- Social Welfare -----------------
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Social Welfare").Id, PostId = posts.First(p => p.PostName == "Supervisor").Id, NotificationNumber = "SW-001", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Social Welfare").Id, PostId = posts.First(p => p.PostName == "Social Worker").Id, NotificationNumber = "SW-002", PostDate = DateTime.UtcNow },
               
               // ----------------- Finance and Accounts -----------------
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Finance and Accounts").Id, PostId = posts.First(p => p.PostName == "Accounts Assistant").Id, NotificationNumber = "FIN-001", PostDate = DateTime.UtcNow },
               
               // ----------------- Forest and Environment -----------------
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Forest and Environment").Id, PostId = posts.First(p => p.PostName == "Forester").Id, NotificationNumber = "FOR-001", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Forest and Environment").Id, PostId = posts.First(p => p.PostName == "Wildlife Guard").Id, NotificationNumber = "FOR-002", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Forest and Environment").Id, PostId = posts.First(p => p.PostName == "Forest Guard").Id, NotificationNumber = "FOR-003", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Forest and Environment").Id, PostId = posts.First(p => p.PostName == "Draftsman").Id, NotificationNumber = "FOR-004", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Forest and Environment").Id, PostId = posts.First(p => p.PostName == "Assistant Information Officer").Id, NotificationNumber = "FOR-005", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Forest and Environment").Id, PostId = posts.First(p => p.PostName == "Lab Assistant").Id, NotificationNumber = "FOR-006", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Forest and Environment").Id, PostId = posts.First(p => p.PostName == "Data Operator").Id, NotificationNumber = "FOR-007", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Forest and Environment").Id, PostId = posts.First(p => p.PostName == "Scientist A").Id, NotificationNumber = "FOR-008", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Forest and Environment").Id, PostId = posts.First(p => p.PostName == "Research Assistant").Id, NotificationNumber = "FOR-009", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Forest and Environment").Id, PostId = posts.First(p => p.PostName == "Junior Scientific Assistant").Id, NotificationNumber = "FOR-010", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Forest and Environment").Id, PostId = posts.First(p => p.PostName == "Junior Environment Engineer").Id, NotificationNumber = "FOR-011", PostDate = DateTime.UtcNow },
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "Forest and Environment").Id, PostId = posts.First(p => p.PostName == "Field Inspector").Id, NotificationNumber = "FOR-012", PostDate = DateTime.UtcNow },
               
               // ----------------- IT -----------------
               new DepartmentPostsDM { DepartmentId = departments.First(d => d.DepartmentName == "IT").Id, PostId = posts.First(p => p.PostName == "Computer Assistant").Id, NotificationNumber = "IT-001", PostDate = DateTime.UtcNow }
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
