using Intern.ServiceModels;
using Intern.ServiceModels.BaseServiceModels;
using Intern.ServiceModels.Exams;
using Intern.ServiceModels.User;
using Intern.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class DepartmentController : ControllerBase
    {
       private readonly DepartmentService _service;
        private readonly DashboardService _dashService;

        public DepartmentController(DepartmentService service,DashboardService dashboardService )
            {
                _service = service;
               _dashService = dashboardService;
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

        [Authorize(Roles = "SystemAdmin")]
        [HttpPost("getall-posts")]
        public async Task<ApiResponse<DepartmentPostsResponseSM>> GetPostsByDepartmentId(int deptId)
       
        {
            var response = await _service.GetPostsByDepartmentId(deptId);
            return ApiResponse<DepartmentPostsResponseSM>.SuccessResponse(response, "Posts fetched successfully");
        }

        [HttpGet("dashboard/{departmentId}")]

        public async Task<ApiResponse<DashboardSM>> GetDashboardDetails(int departmentId)
        {
            if (departmentId <= 0)
                return ApiResponse<DashboardSM>.ErrorResponse("Invalid department id");

            var result = await _dashService.GetDashboardAsync(departmentId);

            if (result == null)
                return ApiResponse<DashboardSM>.ErrorResponse("Dashboard data not found");

            return ApiResponse<DashboardSM>.SuccessResponse(result, "Dashboard fetched successfully");
        }


    }
}
    

