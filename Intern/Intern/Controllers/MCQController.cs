using Intern.ServiceModels;
using Intern.ServiceModels.BaseServiceModels;
using Intern.ServiceModels.Exams;
using Intern.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Intern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MCQController : ControllerBase
    {
        private readonly MCQService _mCQService;

        public MCQController(MCQService mCQService)
        {
            _mCQService = mCQService;
        }

        [HttpGet]
        public async Task<ApiResponse<IEnumerable<MCQsSM>>> GetAll()
        {
            var result = await _mCQService.GetAllAsync();
            return ApiResponse<IEnumerable<MCQsSM>>.SuccessResponse(result, "All MCQs  fetched successfully");
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<MCQsSM>> GetById(int id)
        {
            var result = await _mCQService.GetByIdAsync(id);
            if (result == null)
                return ApiResponse<MCQsSM>.ErrorResponse("MCQ not found");

            return ApiResponse<MCQsSM>.SuccessResponse(result, "MCQ fetched successfully");
        }

        [HttpPost]
        public async Task<ApiResponse<MCQsSM>> Create([FromBody] MCQsSM addmcqs)
        {

            var result = await _mCQService.CreateAsync(addmcqs);
            return ApiResponse<MCQsSM>.SuccessResponse(result, "MCQ Added Successfully");
        }

        [HttpPost("post/{id}")]
        public async Task<ApiResponse<string>> CreateMCQForPost(int id, [FromBody] List<MCQsSM> addmcqs)
        {

            var result = await _mCQService.AddMcqsToPost(id, addmcqs);
            return ApiResponse<string>.SuccessResponse(result, "MCQs Added Successfully");
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<string>> Update(int id, [FromBody] MCQsSM model)  
        {
            model.Id = id;
            var result = await _mCQService.UpdateAsync( model);
            return ApiResponse<string>.SuccessResponse(null, "MCQ updated successfully");
        }


        [HttpDelete("{id}")]
        public async Task<ApiResponse<string>> Delete(int id)
        {
            var success = await _mCQService.DeleteAsync(id);
            if (!success)
                return ApiResponse<string>.ErrorResponse("MCQId not found");

            return ApiResponse<string>.SuccessResponse(null, "MCQ deleted successfully");
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpPost("assign-mcq")]
        public async Task<ApiResponse<string>> AssignMCQToSubjectOrPost([FromBody] MCQSubjectPostSM request)
        {
            await _mCQService.AssignMCQToSubjectOrPostAsync(request);

            return ApiResponse<string>.SuccessResponse(null, "MCQ assigned successfully");
        }

    }
}
