using System.Net;
using Common.Helpers;
using Intern.Common.Helpers;
using Intern.ServiceModels;
using Intern.ServiceModels.BaseServiceModels;
using Intern.ServiceModels.User;
using Intern.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Intern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly TokenHelper _tokenHelper;

        public UserController(UserService userService,TokenHelper tokenHelper)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
        }


        [Authorize(Roles = "SuperAdmin,SystemAdmin")]
        [HttpGet("{id}")]

        public async Task<ApiResponse<UserPerformanceSM>> GetById(int id)
        {
           
            var user = await _userService.GetByIdAsync(id);

            return ApiResponse<UserPerformanceSM>.SuccessResponse(user, "User fetched successfully");
        }

        [Authorize(Roles = "ClientEmployee")]
        [HttpGet("mine")]

        public async Task<ApiResponse<UserPerformanceSM>> GetMineById()
        {
            int userId = _tokenHelper.GetUserIdFromToken();
            var user = await _userService.GetByIdAsync(userId);

            return ApiResponse<UserPerformanceSM>.SuccessResponse(user, "User fetched successfully");
        }

        [Authorize(Roles = "SuperAdmin,SystemAdmin")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<ClientUserSM>> Update(int id, [FromBody] ClientUserSM updatedUser)
        {
            if (updatedUser == null || id <= 0)
                return ApiResponse<ClientUserSM>.ErrorResponse("User not found");

            var result = await _userService.UpdateAsync(id, updatedUser);
            return ApiResponse<ClientUserSM>.SuccessResponse(result, "User updated successfully");
        }

        [Authorize(Roles = "ClientEmployee")]
        [HttpPut("mine")]
        public async Task<ApiResponse<ClientUserSM>> UpdateMine([FromBody] ClientUserSM updatedUser)
        {
            var id = _tokenHelper.GetUserIdFromToken();
            if (updatedUser == null || id <= 0)
                return ApiResponse<ClientUserSM>.ErrorResponse("User not found");

            var result = await _userService.UpdateAsync(id, updatedUser);
            return ApiResponse<ClientUserSM>.SuccessResponse(result, "User updated successfully");
        }
    }
}
