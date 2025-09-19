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
    [AllowAnonymous]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        #region GET ALL

        [HttpGet]
        public async Task<ApiResponse<IEnumerable<NotificationsSM>>> GetAll(int skip = 0, int top = 10)
        {
            var result = await _notificationService.GetAllNotifications(skip, top);
            return ApiResponse<IEnumerable<NotificationsSM>>.SuccessResponse(result, "All notifications fetched successfully");
        }

        [HttpGet("count")]
        public async Task<ApiResponse<int>> GetAllCount()
        {
            var count = await _notificationService.GetAllNotificationsCount();
            return ApiResponse<int>.SuccessResponse(count, "Total notification count fetched successfully");
        }

        [HttpGet("by-dept-post")]
        public async Task<ApiResponse<IEnumerable<NotificationsSM>>> GetByDeptPost(int deptId, int postId, int skip = 0, int top = 10)
        {
            var result = await _notificationService.GetAllNotificationsOfDeptPost(deptId, postId, skip, top);
            return ApiResponse<IEnumerable<NotificationsSM>>.SuccessResponse(result, "Notifications of department post fetched successfully");
        }

        [HttpGet("by-dept-post/count")]
        public async Task<ApiResponse<int>> GetByDeptPostCount(int deptId, int postId)
        {
            var count = await _notificationService.GetAllNotificationsOdDeptPostCount(deptId, postId);
            return ApiResponse<int>.SuccessResponse(count, "Count of notifications for department post fetched successfully");
        }

        #endregion GET ALL

        #region GET SINGLE

        [HttpGet("{id}")]
        public async Task<ApiResponse<NotificationsSM>> GetById(int id)
        {
            var result = await _notificationService.GetNotificationById(id);
            if (result == null)
                return ApiResponse<NotificationsSM>.ErrorResponse("Notification not found");

            return ApiResponse<NotificationsSM>.SuccessResponse(result, "Notification fetched successfully");
        }

        #endregion GET SINGLE

        #region POST

        [Authorize(Roles = "SystemAdmin")]
        [HttpPost()]
        public async Task<ApiResponse<NotificationsSM>> PostNotification([FromBody] NotificationsSM objSM)
        {
            var result = await _notificationService.PostNotification(objSM);
            if (result == null)
                return ApiResponse<NotificationsSM>.ErrorResponse("Input data not found");

            return ApiResponse<NotificationsSM>.SuccessResponse(result, "Notification Added successfully");
        }

        #endregion POST

        #region UPDATE

        [Authorize(Roles = "SystemAdmin")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<NotificationsSM>> Update(int id, [FromBody] NotificationsSM objSM)
        {
            var result = await _notificationService.UpdateById(id, objSM);
            if (result == null)
                return ApiResponse<NotificationsSM>.ErrorResponse("Notification not found");

            return ApiResponse<NotificationsSM>.SuccessResponse(result, "Notification updated successfully");
        }

        #endregion UPDATE

        #region DELETE

        [Authorize(Roles = "SystemAdmin")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<string>> Delete(int id)
        {
            var success = await _notificationService.DeleteAsync(id);
            if (!success)
                return ApiResponse<string>.ErrorResponse("Notification not found");

            return ApiResponse<string>.SuccessResponse(null, "Notification deleted successfully");
        }

        #endregion DELETE
    }
}
