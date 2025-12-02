using Common.Helpers;
using System.Net;
using Intern.ServiceModels.BaseServiceModels;
using Intern.ServiceModels.Exams;
using Intern.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Intern.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectController : ControllerBase
    {
        private readonly SubjectService _subjectService;

        public SubjectController(SubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [Authorize(Roles = "SuperAdmin,SystemAdmin,ClientEmployee")]
        [HttpGet]
        public async Task<ApiResponse<IEnumerable<SubjectSM>>> GetAll()
        {
            var result = await _subjectService.GetAllAsync();
            return ApiResponse<IEnumerable<SubjectSM>>.SuccessResponse(result, "All subjects fetched successfully");
        }

        [Authorize(Roles = "SuperAdmin,SystemAdmin, ClientEmployee")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<SubjectSM>> GetById(int id)
        {
            var result = await _subjectService.GetByIdAsync(id);
            if (result == null)
                return ApiResponse<SubjectSM>.ErrorResponse("Subject not found");

            return ApiResponse<SubjectSM>.SuccessResponse(result, "Subject fetched successfully");
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<ApiResponse<string>> Create([FromBody] SubjectSM subject)
        {
            await _subjectService.CreateAsync(subject);
            return ApiResponse<string>.SuccessResponse(null, "Subject added successfully");
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("{id}")] 
        public async Task<ApiResponse<string>> Update(int id, [FromBody] SubjectSM subject)
        { 
            var result = await _subjectService.UpdateAsync(id,subject);
            return ApiResponse<string>.SuccessResponse(null, result);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<string>> Delete(int id)
        {
            var success = await _subjectService.DeleteAsync(id);
            if (!success)
                return ApiResponse<string>.ErrorResponse("Subject not found");

            return ApiResponse<string>.SuccessResponse(null, "Subject deleted successfully");
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("create-and-assign-subject")]
        public async Task<ApiResponse<string>> CreateAndAssignSubject([FromBody] AddSubjectandAssignSM request)
        {
         
            var result = await _subjectService.CreateAndAssignSubjectAsync(request);
            return ApiResponse<string>.SuccessResponse(null, result);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("remove-subject/{subjectPostId}")]
        public async Task<ApiResponse<string>> RemoveSubjectFromPost(int subjectPostId)
        {
           var success =  await _subjectService.RemoveSubjectsFromPostAsync(subjectPostId);

            if (!success)
                return ApiResponse<string>.ErrorResponse("Failed to remove subject from post");

            return ApiResponse<string>.SuccessResponse(null, "Subject removed from Post successfully");
        }

        [Authorize(Roles = "SuperAdmin,ClientEmployee")]
        [HttpGet("getall-subjects/{postId}")]
        public async Task<ApiResponse<PostSubjectsResponseSM>> GetSubjectsByPostId(int postId)
        {
            var response = await _subjectService.GetSubjectsByPostId(postId);
            return ApiResponse<PostSubjectsResponseSM>.SuccessResponse(response, "Subjects fetched successfully");
        }




    }

}
