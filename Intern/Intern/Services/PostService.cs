using AutoMapper;
using Common.Helpers;
using Intern.Data;
using Intern.DataModels.Exams;
using Intern.ServiceModels;
using Intern.ServiceModels.Exams;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Intern.Services
{
    public class PostService
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;

        public PostService(ApiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PostSM>> GetAllAsync()
        {
            var entities = await _context.Posts.ToListAsync();
            return _mapper.Map<List<PostSM>>(entities);
        }

        public async Task<PostSM?> GetByIdAsync(int id)
        {
            var entity = await _context.Posts.FindAsync(id);
            if (entity == null) return null;
            return _mapper.Map<PostSM>(entity);
        }
        public async Task<PostDetailsSM> GetPostDetails(int id, int userId)
        {
            var postDM = await _context.Posts.FindAsync(id);
            var postSyllabus = await _context.PostSyllabus.Where(x => x.PostId == id).Select(x=>x.SyllabusId).ToListAsync();
            var syllabus = await _context.Syllabus.Where(x => postSyllabus.Contains(x.Id)).ToListAsync();
            var postPapers = await _context.PostPreviousYearPapers.Where(x => x.PostId == id).Select(x=>x.PaperId).ToListAsync();
            var papers = await _context.PreviousYearPapers.Where(x => postPapers.Contains(x.Id)).ToListAsync();
            var notificationsDM = await _context.Notifications.Where(x => x.PostId == id).ToListAsync();
            var userPerformance = await _context.UserTestDetails.Where(x => x.PostId == id && x.UserId == userId).ToListAsync();

            return new PostDetailsSM();

        }

        public async Task<(PostSM? Post, int? DeptPostId)> GetDeptPostDetails(int deptid, int postId)
        {
            var post = await GetByIdAsync(postId);
            if (post == null)
            {
                return (null, null);
            }

            var entity = await _context.DepartmentPosts
                .FirstOrDefaultAsync(x => x.DepartmentId == deptid && x.PostId == postId);

            if (entity == null)
            {
                return (null, null);
            }

            post.NotificationNumber = entity.NotificationNumber;
            post.PostDate = entity.PostDate;

            return (post, entity.Id);
        }


        public async Task<string> CreateAsync(PostSM post)
        {
            bool exists = await _context.Posts
                .AnyAsync(p => p.PostName.ToLower() == post.PostName.Trim().ToLower());

            if (exists)
                throw new AppException("A post with the same name already exists.", HttpStatusCode.Conflict);

            var entity = _mapper.Map<PostDM>(post);
            entity.CreatedOnUtc = DateTime.UtcNow;

            _context.Posts.Add(entity);
            await _context.SaveChangesAsync();
            return null;
        }

        public async Task<string> UpdatePostAsync(int? departmentPostId, PostSM model)
        {
            bool updated = false;

            // ---- Update Posts table (only if PostId given) ----
            if (model.Id > 0)
            {
                var post = await _context.Posts.FindAsync(model.Id);
                if (post == null)
                    throw new AppException("Post not found", HttpStatusCode.NotFound);

                if (!string.IsNullOrWhiteSpace(model.PostName) && model.PostName != "string")
                {
                    post.PostName = model.PostName;
                    updated = true;
                }

                if (!string.IsNullOrWhiteSpace(model.Description) && model.Description != "string")
                {
                    post.Description = model.Description;
                    updated = true;
                }

                if (updated)
                    post.LastModifiedOnUtc = DateTime.UtcNow;
            }

           
            if (departmentPostId.HasValue && departmentPostId.Value > 0)
            {
                var deptPost = await _context.DepartmentPosts
                    .FirstOrDefaultAsync(dp => dp.Id == departmentPostId.Value);

                if (deptPost == null)
                    throw new AppException("Department-Post mapping not found", HttpStatusCode.NotFound);

                if (model.PostDate.HasValue)  
                {
                    deptPost.PostDate = model.PostDate.Value;
                    updated = true;
                }

                if (!string.IsNullOrWhiteSpace(model.NotificationNumber) && model.NotificationNumber != "string")
                {
                    deptPost.NotificationNumber = model.NotificationNumber;
                    updated = true;
                }
            }

            if (!updated)
                throw new AppException("No valid fields provided to update", HttpStatusCode.BadRequest);

            await _context.SaveChangesAsync();
            return "Post updated successfully";
        }



        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new AppException("Invalid Post Id", HttpStatusCode.BadRequest);

            var existing = await _context.Posts.FindAsync(id);
            if (existing == null) return false;

            _context.Posts.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<string> CreateAndAssignPostAsync(AddPostandAssignSM request)
        {
            var department = await _context.Departments.FindAsync(request.DepartmentId);
            if (department == null)
                throw new AppException($"Department with Id {request.DepartmentId} not found.", HttpStatusCode.NotFound);

            bool exists = await _context.Posts
                .AnyAsync(p => p.PostName.ToLower() == request.PostName.Trim().ToLower());

            if (exists)
                throw new AppException("A post with the same name already exists.", HttpStatusCode.Conflict);

            // Step 1: Map to PostDM
            var postEntity = _mapper.Map<PostDM>(request);
            postEntity.CreatedOnUtc = DateTime.UtcNow;

            _context.Posts.Add(postEntity);
            
            
           await _context.SaveChangesAsync(); 

            // Step 2: Map to DepartmentPostsDM
            var deptPostEntity = _mapper.Map<DepartmentPostsDM>(request);
            deptPostEntity.PostId = postEntity.Id;

            _context.DepartmentPosts.Add(deptPostEntity);
            await _context.SaveChangesAsync();

            return "Post created and assigned successfully.";
        }

    }
}
