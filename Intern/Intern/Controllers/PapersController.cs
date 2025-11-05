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
    public class PapersController : ControllerBase
    {
        private readonly PapersService _paperService;

        public PapersController(PapersService papersService)
        {
            _paperService = papersService;
        }

        // 🔹 Get all
        [Authorize(Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        [HttpGet]
        public async Task<ApiResponse<IEnumerable<PapersSM>>> GetAll()
        {
            var result = await _paperService.GetAllAsync();
            return ApiResponse<IEnumerable<PapersSM>>.SuccessResponse(result, "Papers fetched successfully");
        }

        // 🔹 Get by Id
        [Authorize(Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<PapersSM>> GetById(int id)
        {
            var result = await _paperService.GetByIdAsync(id);
            if (result == null)
                return ApiResponse<PapersSM>.ErrorResponse("Paper not found");

            return ApiResponse<PapersSM>.SuccessResponse(result, "Paper fetched successfully");
        }

        // 🔹 Create
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<ApiResponse<string>> Create([FromBody] PapersSM model)
        {
            var message = await _paperService.CreatePaperAsync(model);
            return ApiResponse<string>.SuccessResponse(null, message);
        }


        //Update 
        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<ApiResponse<PapersSM>> UpdatePaper(int paperId, [FromBody] PapersSM model)
        {
            var updatedData = await _paperService.UpdatePaperAsync(paperId, model);
            return ApiResponse<PapersSM>.SuccessResponse(updatedData, "Paper updated successfully");
        }


        // 🔹 Delete
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<string>> Delete(int id)
        {
            var message = await _paperService.DeletePaperAsync(id);
            return ApiResponse<string>.SuccessResponse(null, message);
        }
    }
}
