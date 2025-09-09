using Intern.ServiceModels;
using Intern.ServiceModels.BaseServiceModels;
using Intern.ServiceModels.Exams;
using Intern.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
       private readonly DepartmentService _service;

            public DepartmentController(DepartmentService service)
            {
                _service = service;
            }
      
        [HttpGet]
        public async Task<ApiResponse<IEnumerable<DepartmentSM>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return ApiResponse<IEnumerable<DepartmentSM>>.SuccessResponse(result, "All Departments fetched successfully");
        }


        [HttpGet("{id}")]
        public async Task<ApiResponse<DepartmentSM>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return ApiResponse<DepartmentSM>.ErrorResponse("Department not found");

            return ApiResponse<DepartmentSM>.SuccessResponse(result, "Department fetched successfully");
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpPost]
        public async Task<ApiResponse<string>> Create([FromBody] DepartmentSM addDepartmentSM)
        {

            await _service.CreateAsync(addDepartmentSM);
            return ApiResponse<string>.SuccessResponse(null, "Department Added Successfully");
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<string>> Update(int id, [FromBody] DepartmentSM updateDepartmentSM)
        {
            var success = await _service.UpdateAsync(updateDepartmentSM);
            if (!success)
                return ApiResponse<string>.ErrorResponse("Department not found");

            return ApiResponse<string>.SuccessResponse(null, "Department updated successfully");
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<string>> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
                return ApiResponse<string>.ErrorResponse("Department not found");

            return ApiResponse<string>.SuccessResponse(null, "Department deleted successfully");
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpPost("assign-posts")]
        public async Task<ApiResponse<string>> AssignPostsToDepartment([FromBody] DepartmentPostsSM request)
        {
            
            await _service.AssignPostToDepartmentAsync(request);

            return ApiResponse<string>.SuccessResponse(null, "Posts assigned successfully");
        }
        [Authorize(Roles ="SystemAdmin")]
        [HttpPost("remove-posts")]

        public async Task<ApiResponse<string>> RemovepostsfromDepartment([FromBody] RemovepostsfromDepartmentSM removepostsfromDepartment)
        {
            await _service.RemovepostsfromDepartmentAsync(removepostsfromDepartment);
            return ApiResponse<string>.SuccessResponse(null, "Post removed from Department Successfully");
        }

       


    }
}
    

