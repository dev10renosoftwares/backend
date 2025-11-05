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
    public class SyllabusController : ControllerBase
    {
        private readonly SyllabusService _syllabusService;

        public SyllabusController(SyllabusService syllabusService)
        {
            _syllabusService = syllabusService;
        }

        // 🔹 Get all
        [Authorize(Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        [HttpGet]
        public async Task<ApiResponse<IEnumerable<SyllabusSM>>> GetAll()
        {
            var result = await _syllabusService.GetAllAsync();
            return ApiResponse<IEnumerable<SyllabusSM>>.SuccessResponse(result, "Syllabuses fetched successfully");
        }

        // 🔹 Get by Id
        [Authorize(Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<SyllabusSM>> GetById(int id)
        {
            var result = await _syllabusService.GetByIdAsync(id);
            if (result == null)
                return ApiResponse<SyllabusSM>.ErrorResponse("Syllabus not found");

            return ApiResponse<SyllabusSM>.SuccessResponse(result, "Syllabus fetched successfully");
        }

        // 🔹 Create
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<ApiResponse<string>> Create([FromBody] SyllabusSM model)
        {
            var message = await _syllabusService.CreateSyllabusAsync(model);
            return ApiResponse<string>.SuccessResponse(null, message);
        }

        // 🔹 Update
        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<ApiResponse<SyllabusSM>> UpdateSyllabus(int syllabusId, [FromBody] SyllabusSM model)
        {
            var updatedData = await _syllabusService.UpdateSyllabusAsync(syllabusId, model);
            return ApiResponse<SyllabusSM>.SuccessResponse(updatedData, "Syllabus updated successfully");
        }

        // 🔹 Delete
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<string>> Delete(int id)
        {
            var message = await _syllabusService.DeleteSyllabusAsync(id);
            return ApiResponse<string>.SuccessResponse(null, message);
        }
    }
}
