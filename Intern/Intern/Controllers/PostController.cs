using Intern.ServiceModels.BaseServiceModels;
using Intern.ServiceModels;
using Intern.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Intern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;

        public PostController(PostService postService)
        {
            _postService = postService;
        }
        [HttpGet]
        public async Task<ApiResponse<IEnumerable<PostSM>>> GetAll()
        {
            var result = await _postService.GetAllAsync();
            return ApiResponse<IEnumerable<PostSM>>.SuccessResponse(result, "All Posts fetched successfully");
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<PostSM>> GetById(int id)
        {
            var result = await _postService.GetByIdAsync(id);
            if (result == null)
                return ApiResponse<PostSM>.ErrorResponse("Post not found");

            return ApiResponse<PostSM>.SuccessResponse(result, "Post fetched successfully");
        }


        [Authorize(Roles = "SystemAdmin")]
        [HttpPost]
        public async Task<ApiResponse<string>> Create([FromBody] AddPostSM addPostSM)
        {
            await _postService.CreateAsync(addPostSM);
            return ApiResponse<string>.SuccessResponse(null, "Post added successfully");
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<string>> Update(int id, [FromBody] AddPostSM updatePostSM)
        {
            var success = await _postService.UpdateAsync(id, updatePostSM);
            if (!success)
                return ApiResponse<string>.ErrorResponse("Post not found");

            return ApiResponse<string>.SuccessResponse(null, "Post updated successfully");
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<string>> Delete(int id)
        {
            var success = await _postService.DeleteAsync(id);
            if (!success)
                return ApiResponse<string>.ErrorResponse("Post not found");

            return ApiResponse<string>.SuccessResponse(null, "Post deleted successfully");
        }
    }
}

