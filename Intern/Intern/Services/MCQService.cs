using System.Net;
using AutoMapper;
using Common.Helpers;
using Intern.Common;
using Intern.Common.Helpers;
using Intern.Data;
using Intern.DataModels.Enums;
using Intern.DataModels.Exams;
using Intern.DataModels.User;
using Intern.ServiceModels;
using Intern.ServiceModels.Enums;
using Intern.ServiceModels.Exams;
using Intern.ServiceModels.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Intern.Services
{
    public class MCQService
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;
        private readonly PostService _postSevice;        
        private readonly DepartmentService _deptService;
        private readonly TokenHelper _tokenHelper;
        private readonly SubjectService _subjectService;
        private readonly ExamConfig _examConfig;

        public MCQService(ApiDbContext context,IMapper mapper, PostService postService, DepartmentService deptService,TokenHelper tokenHelper, IOptions<ExamConfig> examConfig,SubjectService subjectService)
        {
            _context = context;
            _mapper = mapper;
            _postSevice = postService;
            _deptService = deptService;
            _tokenHelper = tokenHelper;
            _subjectService = subjectService;
            _examConfig = examConfig.Value;
        }

        public async Task<List<MCQsSM>> GetAllAsync()
        {
            var mcqs = await _context.MCQs.ToListAsync();
            return _mapper.Map<List<MCQsSM>>(mcqs);
        }

        public async Task<MCQsSM?> GetByIdAsync(int id)
        {
            var entity = await _context.MCQs.FindAsync(id);

            if (entity == null)
                return null;

            return _mapper.Map<MCQsSM>(entity);
        }

        public async Task<MCQsSM> CreateAsync(MCQsSM addmcqs)
        {
           
            var exists = await _context.MCQs
                .FirstOrDefaultAsync(mcq =>mcq.Question == addmcqs.Question);

            if (exists != null)
            {
                return _mapper.Map<MCQsSM>(exists);
            }

           
            var entity = _mapper.Map<MCQsDM>(addmcqs);
            entity.CreatedOnUtc = DateTime.UtcNow;

            await _context.MCQs.AddAsync(entity);
            if(await _context.SaveChangesAsync() > 0)
            {
                return await GetByIdAsync(entity.Id);
            }

            return null;
        }

        public async Task<string> AddMcqsToPost(int postId, List<MCQsSM> objSM)
        {
            var existingPost = await _postSevice.GetByIdAsync(postId);
            if (existingPost == null)
            {
                throw new AppException("Cannot add MCQ as Post Id is not found");
            }

            if (objSM == null || objSM.Count == 0)
            {
                throw new AppException("MCQs not found");
            }

            // ✅ Begin transaction
            var loginId = _tokenHelper.GetLoginIdFromToken();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var sm in objSM)
                {
                    sm.CreatedBy = loginId;
                    var added = await CreateAsync(sm);  
                    if (added != null)
                    {
                        var existingRelation = await _context.MCQPostSubjects.Where(x => x.MCQId == added.Id && x.PostId == postId).FirstOrDefaultAsync();
                        if(existingRelation != null)
                        {
                            continue;
                        }
                        var mcqPost = new MCQPostSubjectDM
                        {
                            MCQId = added.Id,
                            PostId = postId,
                            MCQType = McqTypeDM.Post
                        };

                        await _context.MCQPostSubjects.AddAsync(mcqPost);
                        await _context.SaveChangesAsync();
                    }
                }

                // ✅ Commit transaction
                await transaction.CommitAsync();
                return "true";
            }
            catch (Exception e)
            {
                // ❌ Rollback transaction
                await transaction.RollbackAsync();
                throw e;
            }
        }

        public async Task<string> AddMcqsToSubject(int subjectId, List<MCQsSM> objSM)
        {
            var existingSubject = await _subjectService.GetByIdAsync(subjectId);
            if (existingSubject == null)
            {
                throw new AppException("Cannot add MCQ as Subject Id is not found");
            }

            if (objSM == null || objSM.Count == 0)
            {
                throw new AppException("MCQs not found");
            }

            var loginId = _tokenHelper.GetLoginIdFromToken();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var sm in objSM)
                {
                    sm.CreatedBy = loginId;
                    var added = await CreateAsync(sm);

                    if (added != null)
                    {
                        // ✅ Check if MCQ already linked to Subject
                        var existingRelation = await _context.MCQPostSubjects
                            .FirstOrDefaultAsync(x => x.MCQId == added.Id && x.SubjectId == subjectId);

                        if (existingRelation != null)
                        {
                            continue;
                        }

                        // ✅ Create relation with Subject
                        var mcqSubject = new MCQPostSubjectDM
                        {
                            MCQId = added.Id,
                            SubjectId = subjectId,
                            MCQType = McqTypeDM.Subject
                        };

                        await _context.MCQPostSubjects.AddAsync(mcqSubject);
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();
                return "true";
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw e;
            }
        }


        public async Task<string> UpdateAsync(MCQsSM updatedMcq)
        {
            var entity = await _context.MCQs.FindAsync(updatedMcq.Id);

            if (entity == null)
                throw new AppException($"MCQ with Id {updatedMcq.Id} not found.", HttpStatusCode.NotFound);


            if (!string.IsNullOrEmpty(updatedMcq.Question))
                entity.Question = updatedMcq.Question;

            if (!string.IsNullOrEmpty(updatedMcq.OptionA))
                entity.OptionA = updatedMcq.OptionA;

            if (!string.IsNullOrEmpty(updatedMcq.OptionB))
                entity.OptionB = updatedMcq.OptionB;

            if (!string.IsNullOrEmpty(updatedMcq.OptionC))
                entity.OptionC = updatedMcq.OptionC;

            if (!string.IsNullOrEmpty(updatedMcq.OptionD))
                entity.OptionD = updatedMcq.OptionD;

            if (!string.IsNullOrEmpty(updatedMcq.Answer))
                entity.Answer = updatedMcq.Answer;

            if (!string.IsNullOrEmpty(updatedMcq.Explanation))
                entity.Explanation = updatedMcq.Explanation;

            _mapper.Map(updatedMcq, entity);

           


            entity.LastModifiedOnUtc = DateTime.UtcNow;
           

            await _context.SaveChangesAsync();

            return "MCQ updated successfully";
        }
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new AppException("Invalid MCQ Id", HttpStatusCode.BadRequest);

            var existing = await _context.MCQs.FindAsync(id);
            if (existing == null) return false;

            _context.MCQs.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
        //incomplete...
        public async Task<bool> AssignMCQToSubjectOrPostAsync(MCQSubjectPostSM objSM)
        {
            // Validate required field
            if (objSM.MCQId <= 0)
                throw new AppException("MCQId is required", HttpStatusCode.BadRequest);

            // Determine McqType based on SubjectId and PostId
            if (objSM.SubjectId.HasValue && objSM.PostId.HasValue)
            {
                objSM.McqType = McqTypeSM.General;
            }
            else if (objSM.PostId.HasValue && !objSM.SubjectId.HasValue)
            {
                objSM.McqType = McqTypeSM.Post;
            }
            else if (objSM.SubjectId.HasValue && !objSM.PostId.HasValue)
            {
                objSM.McqType = McqTypeSM.Subject;
            }
            else
            {
                throw new AppException("Either SubjectId, PostId or both must be provided", HttpStatusCode.BadRequest);
            }

            // Validate subject if given
            if (objSM.SubjectId.HasValue)
            {
                var subject = await _context.Subjects.FindAsync(objSM.SubjectId.Value);
                if (subject == null)
                    throw new AppException($"Subject with Id {objSM.SubjectId.Value} not found", HttpStatusCode.NotFound);
            }

            // Validate post if given
            if (objSM.PostId.HasValue)
            {
                var post = await _context.Posts.FindAsync(objSM.PostId.Value);
                if (post == null)
                    throw new AppException($"Post with Id {objSM.PostId.Value} not found", HttpStatusCode.NotFound);
            }

            // Validate MCQ existence
            var mcq = await _context.MCQs.FindAsync(objSM.MCQId);
            if (mcq == null)
                throw new AppException($"MCQ with Id {objSM.MCQId} not found", HttpStatusCode.NotFound);

            // Prevent duplicates
            var exists = await _context.MCQPostSubjects
                .FirstOrDefaultAsync(x =>
                    x.MCQId == objSM.MCQId &&
                    x.SubjectId == objSM.SubjectId &&
                    x.PostId == objSM.PostId &&
                    x.MCQType == (McqTypeDM)objSM.McqType);

            if (exists != null)
                throw new AppException("MCQ is already assigned", HttpStatusCode.Conflict);

            // Map and save
            var dm = _mapper.Map<MCQPostSubjectDM>(objSM);
            await _context.MCQPostSubjects.AddAsync(dm);

            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }

            throw new AppException("Something went wrong while assigning MCQ", HttpStatusCode.BadRequest);
        }

        public async Task<MockTestQuestionsSM> GetMCQsByDepartmentAndPostAsync(int userId, int departmentId, int postId)
        {

            // ✅ Check max tests per user
            int userTestCount = await _context.UserTestDetails
                .CountAsync(t => t.UserId == userId);

            if (userTestCount >= _examConfig.MaxTestsPerUser)
                throw new AppException(
                    $"You cannot attempt more than {_examConfig.MaxTestsPerUser} tests.",
                    HttpStatusCode.Forbidden
                );

            string LoginId = _tokenHelper.GetLoginIdFromToken();
            // 1️⃣ Validate Department and Post
            var existingDept = await _deptService.GetByIdAsync(departmentId);
            var existingPost = await _postSevice.GetByIdAsync(postId);

            if (existingDept == null || existingPost == null)
                throw new AppException("Department or post is not found", HttpStatusCode.NotFound);

            // 2️⃣ Validate Post belongs to Department
            var isValidPost = await _context.DepartmentPosts
                .AnyAsync(dp => dp.DepartmentId == departmentId && dp.PostId == postId);

            if (!isValidPost)
                throw new AppException("The specified post does not belong to this department.", HttpStatusCode.Conflict);

            // 3️⃣ Fetch MCQs directly with join and random order (single query)
            var mcqs = await (from m in _context.MCQs
                              join mps in _context.MCQPostSubjects
                                  on m.Id equals mps.MCQId
                              where mps.PostId == postId
                              orderby Guid.NewGuid() 
                              select m)
                             .Take(50) 
                             .ToListAsync();

            // 4️⃣ Map to Service Model
            var mcqsSM = _mapper.Map<List<MCQsSM>>(mcqs);

            // 5️⃣ Hide answers and explanations for exam purposes    
            foreach (var mcq in mcqsSM)
            {
                mcq.Answer = null;
                mcq.Explanation = null;
            }
            var userExamDetails = new UserTestDetailsDM
            {
                UserId = userId,
                TestTaken = true,
                TotalQuestions = mcqsSM.Count,
                PostId = postId,
                SubjectId = null,
                MCQType = McqTypeDM.Post,
                CreatedBy = LoginId,
                CreatedOnUtc = DateTime.UtcNow,
            };
            await _context.UserTestDetails.AddAsync(userExamDetails);
            await _context.SaveChangesAsync();

            return new MockTestQuestionsSM
            {
                UserTestId = userExamDetails.Id,
                Questions = mcqsSM,
            };
                
        }

        public async Task<AnswerResultSM> IsCorrectAnswer(MCQsSM mcq)
        {
            var existingMCQ = await GetByIdAsync(mcq.Id);
            if (existingMCQ == null)
            {
                return AnswerResultSM.Other;
            }
            if(string.IsNullOrEmpty(mcq.Answer))
            {
                return AnswerResultSM.NotAttempted;  
            }

            if(existingMCQ.Answer == mcq.Answer)
            {
                return AnswerResultSM.Right;
            }
            return AnswerResultSM.Wrong;
        }
          
        public async Task<UserTestDetailsSM> GetUserTestById(int userTestId)
        {
            var dm = await _context.UserTestDetails.FindAsync(userTestId);
            if (dm == null) { return null; }

            return _mapper.Map<UserTestDetailsSM>(dm);
        }

        public async Task<UserTestDetailsSM> GetTestResults(int id, MockTestQuestionsSM objSM)
        {
            var existingUser = await _context.ClientUsers
               .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

            if (existingUser == null)
            {
                throw new AppException("User details not Found", HttpStatusCode.Unauthorized);
            }

            try
            {
                var existingUserTest = await _context.UserTestDetails
                .FirstOrDefaultAsync(t => t.Id == objSM.UserTestId);
                if (existingUserTest == null)
                    throw new AppException("User test not found", HttpStatusCode.NotFound);

                if (objSM.Questions.Count == 0)
                    throw new AppException("No answers submitted", HttpStatusCode.BadRequest);

                if (objSM.Questions.Count != existingUserTest.TotalQuestions)
                    throw new AppException("Malformed submission: question count mismatch", HttpStatusCode.BadRequest);

                // 2️⃣ Calculate results
                int rightAnswers = 0, wrongAnswers = 0, notAttempted = 0;

                foreach (var mcq in objSM.Questions)
                {
                    var result = await IsCorrectAnswer(mcq); // returns Right/Wrong/NotAttempted
                    switch (result)
                    {
                        case AnswerResultSM.Right:
                            rightAnswers++;
                            break;
                        case AnswerResultSM.Wrong:
                            wrongAnswers++;
                            break;
                        case AnswerResultSM.NotAttempted:
                            notAttempted++;
                            break;
                        default:

                            break;
                    }
                }

                string LoginId = _tokenHelper.GetLoginIdFromToken();
                // 3️⃣ Update test record
                existingUserTest.RightAnswered = rightAnswers;
                existingUserTest.WrongAnswered = wrongAnswers;
                existingUserTest.NotAttempted = notAttempted;
                existingUserTest.TestSubmitted = true;
                existingUserTest.LastModifiedBy = LoginId;
                existingUserTest.LastModifiedOnUtc = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // 4️⃣ Map back to SM
                return _mapper.Map<UserTestDetailsSM>(existingUserTest);
            }
            catch(Exception e)
            {
                throw;
            }

            
        }


        public async Task<MockTestQuestionsSM> GetMCQsBySubjectAsync(int userId, int subjectId)
        {
            // 1️⃣ Check max tests per user
            int userTestCount = await _context.UserTestDetails
                .CountAsync(t => t.UserId == userId);

            if (userTestCount >= _examConfig.MaxTestsPerUser)
                throw new AppException(
                    $"You cannot attempt more than {_examConfig.MaxTestsPerUser} tests.",
                    HttpStatusCode.Forbidden
                );

            string loginId = _tokenHelper.GetLoginIdFromToken();

            // 2️⃣ Validate Subject
            var existingSubject = await _subjectService.GetByIdAsync(subjectId);
            if (existingSubject == null)
                throw new AppException("Subject not found", HttpStatusCode.NotFound);

            // 3️⃣ Fetch MCQs by SubjectId
            var mcqs = await (from m in _context.MCQs
                              join mps in _context.MCQPostSubjects on m.Id equals mps.MCQId
                              where mps.SubjectId == subjectId
                              orderby Guid.NewGuid()
                              select m)
                             .Take(50)
                             .ToListAsync();

            if (!mcqs.Any())
                throw new AppException("No MCQs found for this subject.", HttpStatusCode.NotFound);

            // 4️⃣ Map to SM and hide answers
            var mcqsSM = _mapper.Map<List<MCQsSM>>(mcqs);
            foreach (var mcq in mcqsSM)
            {
                mcq.Answer = null;
                mcq.Explanation = null;
            }

            // 5️⃣ Save User Test Details
            var userExamDetails = new UserTestDetailsDM
            {
                UserId = userId,
                TestTaken = true,
                TotalQuestions = mcqsSM.Count,
                PostId = null,              
                SubjectId = subjectId,      
                MCQType = McqTypeDM.Subject,
                CreatedBy = loginId,
                CreatedOnUtc = DateTime.UtcNow,
            };
            await _context.UserTestDetails.AddAsync(userExamDetails);
            await _context.SaveChangesAsync();

            // 6️⃣ Return response
            return new MockTestQuestionsSM
            {
                UserTestId = userExamDetails.Id,
                Questions = mcqsSM,
            };
        }



    }
}
