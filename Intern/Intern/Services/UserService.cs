using AutoMapper;
using Common.Helpers;
using Intern.Common;
using Intern.Common.Helpers;
using Intern.Data;
using Intern.DataModels.Enums;
using Intern.DataModels.User;
using Intern.ServiceModels;
using Intern.ServiceModels.Enums;
using Intern.ServiceModels.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;

namespace Intern.Services
{
    public class UserService
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;
        private readonly ImageHelper _imageHelper;
        private readonly TokenHelper _tokenHelper;
        private readonly ExamConfig _examConfig;

        public UserService(ApiDbContext context, IMapper mapper,ImageHelper imageHelper,IOptions<ExamConfig> maxtests ,TokenHelper tokenHelper)
        {
            _context = context;
            _mapper = mapper;
            _imageHelper = imageHelper;
            _tokenHelper = tokenHelper;
            _examConfig = maxtests.Value;
        }

        public async Task<UserPerformanceSM> GetByIdAsync(int id)
        {
            // First check ClientUsers
            var clientUser = await _context.ClientUsers.FindAsync(id);
            if (clientUser != null)
            {

                var sm = _mapper.Map<UserPerformanceSM>(clientUser);

                if (!string.IsNullOrEmpty(clientUser.ImagePath))
                {
                    // convert image path to base64
                    sm.ImageBase64 = _imageHelper.ConvertFileToBase64(clientUser.ImagePath);
                }
                sm.UserName = clientUser.LoginId;
                sm.Password = null;

                // =============================
                // Fetch user test performance
                // =============================
                var takenTests = await _context.UserTestDetails
                        .Where(t => t.UserId == clientUser.Id && t.TestTaken)
                        .Include(t => t.Post) // Assuming navigation property to Post
                        .ToListAsync();

                int maxTests = _examConfig.MaxTestsPerUser;

                sm.TotalTestsTaken = takenTests.Count;
                sm.TotalRemainingTests = maxTests - sm.TotalTestsTaken;

                if (takenTests.Any(t => t.TestSubmitted))
                {
                    var submittedTests = takenTests.Where(t => t.TestSubmitted);

                    // Calculate marks with negative marking for each test
                    var testWithMarks = submittedTests
                        .Select(t =>
                        {
                            double positiveMarks = (double)t.RightAnswered / t.TotalQuestions * 100;
                            double negativeMarks = (double)t.WrongAnswered * _examConfig.NegativeMarkPerQuestion;
                            double totalMarks = Math.Max(positiveMarks - negativeMarks, 0);
                            return new
                            {
                                Test = t,
                                Marks = totalMarks
                            };
                        })
                        .ToList();

                    // Average marks
                    sm.AverageMarks = testWithMarks.Average(x => x.Marks);

                    // Top performance
                    var top = testWithMarks.OrderByDescending(x => x.Marks).First();

                    // Map only PostName using AutoMapper
                    sm.TopPerformance = new UserPerformanceSM.TopPerformanceSM
                    {
                        Post = new PostSM
                        {
                            PostName = top.Test.Post.PostName  // ✅ Only map PostName
                        },
                        Marks = top.Marks
                    };

                }
                else
                {
                    sm.AverageMarks = 0;
                    sm.TopPerformance = new UserPerformanceSM.TopPerformanceSM
                    {
                        Post = null,
                        Marks = 0
                    };
                }

                return sm;



            }
            // If neither found
            throw new AppException("User not found", HttpStatusCode.NotFound);
        }




        public async Task<ClientUserSM> UpdateAsync(int id, ClientUserSM objSM)
        {
            var loginId = _tokenHelper.GetLoginIdFromToken();
            var userDM = await _context.ClientUsers.FindAsync(id);
            if (userDM == null)
            {
                throw new AppException("User not found", HttpStatusCode.NotFound);
            }
            if(objSM == null)
            {
                throw new AppException("Please provide details to update", HttpStatusCode.BadRequest);
            }
            objSM.Id = userDM.Id;
            objSM.Email = userDM.Email;
            objSM.Role = (UserRoleSM)userDM.Role;
            objSM.UserName = userDM.LoginId;
            objSM.Password = userDM.Password;
            objSM.IsEmailConfirmed = userDM.IsEmailConfirmed;
            objSM.IsActive = userDM.IsActive;
            objSM.IsMobileNumberConfirmed = userDM.IsMobileNumberConfirmed;

            // ✅ Map incoming values into the tracked entity
            _mapper.Map(objSM, userDM);

            userDM.LastModifiedBy = loginId;
            userDM.LastModifiedOnUtc = DateTime.UtcNow;

            // ✅ Handle image
            if (!string.IsNullOrEmpty(objSM.ImageBase64))
            {
                var directory = Directory.GetCurrentDirectory();
                var newPath = Path.Combine(directory, @"Images");

                // delete old image if it exists
                if (!string.IsNullOrEmpty(userDM.ImagePath) && File.Exists(userDM.ImagePath))
                {
                    File.Delete(userDM.ImagePath);
                }

                // save new image
                userDM.ImagePath = await _imageHelper.SaveBase64ImageAsync(objSM.ImageBase64, newPath);
            }

            
            try {
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }

            // ✅ Prepare response
            var userSM = _mapper.Map<ClientUserSM>(userDM);

            // Convert existing file path to Base64 if file exists
            if (!string.IsNullOrEmpty(userDM.ImagePath) && File.Exists(userDM.ImagePath))
            {
                userSM.ImageBase64 = Convert.ToBase64String(File.ReadAllBytes(userDM.ImagePath));
            }

            return userSM;

        }



    }
}
