using System.Net;
using Common.Helpers;
using Intern.Common.Helpers;
using Intern.Data;
using Intern.ServiceModels;
using Intern.ServiceModels.BaseServiceModels;
using Intern.ServiceModels.Exams;
using Intern.ServiceModels.User;
using Intern.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Intern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class MCQController : ControllerBase
    {
        private readonly MCQService _mCQService;
        private readonly TokenHelper _tokenHelper;
        private readonly ApiDbContext _context;

        public MCQController(MCQService mCQService,TokenHelper tokenHelper,ApiDbContext context)
        {
            _mCQService = mCQService;
            _tokenHelper = tokenHelper;
            _context = context;
        }
        [Authorize(Roles = "SuperAdmin,SystemAdmin")]
        [HttpGet]
        public async Task<ApiResponse<IEnumerable<MCQsSM>>> GetAll()
        {
            var result = await _mCQService.GetAllAsync();
            return ApiResponse<IEnumerable<MCQsSM>>.SuccessResponse(result, "All MCQs  fetched successfully");
        }

        [Authorize(Roles = "SuperAdmin,SystemAdmin")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<MCQsSM>> GetById(int id)
        {
            var result = await _mCQService.GetByIdAsync(id);
            if (result == null)
                return ApiResponse<MCQsSM>.ErrorResponse("MCQ not found");

            return ApiResponse<MCQsSM>.SuccessResponse(result, "MCQ fetched successfully");
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<ApiResponse<MCQsSM>> Create([FromBody] MCQsSM addmcqs)
        {

            var result = await _mCQService.CreateAsync(addmcqs);
            return ApiResponse<MCQsSM>.SuccessResponse(result, "MCQ Added Successfully");
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("post/{id}")]
        public async Task<ApiResponse<string>> CreateMCQForPost(int id, [FromBody] List<MCQsSM> addmcqs)
        {

            var result = await _mCQService.AddMcqsToPost(id, addmcqs);
            return ApiResponse<string>.SuccessResponse(result, "MCQs Added Successfully");
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("subject/{id}")]
        public async Task<ApiResponse<string>> CreateMCQForSubject(int id, [FromBody] List<MCQsSM> addmcqs)
        {
            var result = await _mCQService.AddMcqsToSubject(id, addmcqs);
            return ApiResponse<string>.SuccessResponse(result, "MCQs Added Successfully to Subject");
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<string>> Update(int id, [FromBody] MCQsSM model)  
        {
            model.Id = id;
            var result = await _mCQService.UpdateAsync( model);
            return ApiResponse<string>.SuccessResponse(null, "MCQ updated successfully");
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<string>> Delete(int id)
        {
            var success = await _mCQService.DeleteAsync(id);
            if (!success)
                return ApiResponse<string>.ErrorResponse("MCQId not found");

            return ApiResponse<string>.SuccessResponse(null, "MCQ deleted successfully");
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("assign-mcq")]
        public async Task<ApiResponse<string>> AssignMCQToSubjectOrPost([FromBody] MCQSubjectPostSM request)
        {
            await _mCQService.AssignMCQToSubjectOrPostAsync(request);

            return ApiResponse<string>.SuccessResponse(null, "MCQ assigned successfully");
        }

        [Authorize(Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        [HttpGet("get-mcqs")]
        public async Task<ApiResponse<MockTestQuestionsSM>> GetAllMCQs(int postId, int departmentId)
        {
            int userId = _tokenHelper.GetUserIdFromToken();
            var result = await _mCQService.GetMCQsByDepartmentAndPostAsync(userId, departmentId, postId);
            return ApiResponse<MockTestQuestionsSM>.SuccessResponse(result, "MCQs fetched successfully");
        }

        [Authorize(Roles = "ClientEmployee")]
        [HttpPost("results")]
        public async Task<ApiResponse<UserTestDetailsSM>> GetTestResults([FromBody] MockTestQuestionsSM answers)
        {
            var userId = _tokenHelper.GetUserIdFromToken();

            var existingUser = await _context.ClientUsers
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

            if (existingUser == null)
            {
                throw new AppException("User is not authorized", HttpStatusCode.Unauthorized);
            }
  
            var result = await _mCQService.GetTestResults(answers);

            return ApiResponse<UserTestDetailsSM>.SuccessResponse(result, "MCQs Result fetched successfully");
        }

        [Authorize(Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        [HttpGet("get-mcqs/{subjectId}")]
        public async Task<ApiResponse<MockTestQuestionsSM>> GetMCQsBySubject(int subjectId)
        {
            int userId = _tokenHelper.GetUserIdFromToken(); ; 
            var result = await _mCQService.GetMCQsBySubjectAsync(userId, subjectId);
            return ApiResponse<MockTestQuestionsSM>.SuccessResponse(result, "MCQs fetched successfully");
        }





    }
}
  