using Intern.ServiceModels.BaseServiceModels;
using Intern.ServiceModels;
using Intern.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Intern.ServiceModels.Exams;
using Common.Helpers;
using System.Net;
using Intern.Common.Helpers;
using Intern.DataModels.Exams;

namespace Intern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly TokenHelper _tokenHelper;

        public PostController(PostService postService,TokenHelper tokenHelper)
        {
            _postService = postService;
            _tokenHelper = tokenHelper;
        }

        [Authorize(Roles = "SuperAdmin,SystemAdmin")]
        [HttpGet]
        public async Task<ApiResponse<IEnumerable<PostSM>>> GetAll()
        {
            var result = await _postService.GetAllAsync();
            return ApiResponse<IEnumerable<PostSM>>.SuccessResponse(result, "All Posts fetched successfully");
        }

        [Authorize(Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<PostSM>> GetById(int id)
        {
            var result = await _postService.GetByIdAsync(id);
            if (result == null)
                return ApiResponse<PostSM>.ErrorResponse("Post not found");

            return ApiResponse<PostSM>.SuccessResponse(result, "Post fetched successfully");
        }
        
        [Authorize(Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        [HttpGet("details/{postId}")]
        public async Task<ApiResponse<PostDetailsSM>> GetPostDetails(int postId)
        {
            int userId = _tokenHelper.GetUserIdFromToken();

            var result = await _postService.GetPostDetailsAsync(postId,userId);
            if (result == null)
                return ApiResponse<PostDetailsSM>.ErrorResponse("Post not found");

            return ApiResponse<PostDetailsSM>.SuccessResponse(result, "Post fetched successfully");
        }

       
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<ApiResponse<string>> Create([FromBody] PostSM addPostSM)
        {
            await _postService.CreateAsync(addPostSM);
            return ApiResponse<string>.SuccessResponse(null, "Post added successfully");
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]        
        public async Task<ApiResponse<string>> UpdatePost( int? departmentPostId, [FromBody] PostSM postsm)
        {
            var result = await _postService.UpdatePostAsync( departmentPostId, postsm);
            return ApiResponse<string>.SuccessResponse(null, result);
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<string>> Delete(int id)
        {
            var success = await _postService.DeleteAsync(id);
            if (!success)
                return ApiResponse<string>.ErrorResponse("Post not found");

            return ApiResponse<string>.SuccessResponse(null, "Post deleted successfully");
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("create-and-assign-post")]
        public async Task<ApiResponse<string>> CreateAndAssignPost([FromBody] AddPostandAssignSM request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();

                throw new AppException(string.Join(" | ", errors), HttpStatusCode.BadRequest);
            }

            
            
            var result = await _postService.CreateAndAssignPostAsync(request);

            return ApiResponse<string>.SuccessResponse(null, result);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("assign-previouspapers")]
        public async Task<ApiResponse<string>> AssignPreviousPapers([FromBody] List<PostPapersSM> model)
         {
             await _postService.AssignPreviousPapersAsync(model);
            return ApiResponse<string>.SuccessResponse(null, "Previous year papers assigned successfully");
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("assign-syllabus")]
        public async Task<ApiResponse<string>> AssignSyllabus([FromBody] List<PostSyllabusSM> model)
        {
            await _postService.AssignPostSyllabusAsync(model);
            return ApiResponse<string>.SuccessResponse(null, "Syllabus assigned successfully");
        }




    }
}

