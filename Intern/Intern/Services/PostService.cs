using AutoMapper;
using Common.Helpers;
using Intern.Data;
using Intern.DataModels.Exams;
using Intern.ServiceModels;
using Intern.ServiceModels.Exams;
using Intern.ServiceModels.User;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Intern.Services
{
    public class PostService
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;
        private readonly SyllabusService _syllabusService;
        private readonly PapersService _papersService;
       

        public PostService(ApiDbContext context, IMapper mapper,SyllabusService syllabusService,PapersService papersService)
        {
            _context = context;
            _mapper = mapper;
            _syllabusService = syllabusService;
            _papersService = papersService;
           
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
        public async Task<PostDetailsSM> GetPostDetailsAsync(int postid, int userId)
        {
            var postSM = await GetByIdAsync(postid);
            var syllabusListSM = await _syllabusService.GetSyllabusByPostIdAsync(postid);
            var papersListSM = await _papersService.GetPapersByPostIdAsync(postid);
            var notificationsListSM = await GetNotificationsByPostIdAsync(postid);
            var userPerformance = await GetUserPerformanceAsync(postid, userId);

            var postDetailsSM = new PostDetailsSM
            {
                Post = postSM,
                Syllabus = syllabusListSM,
                PreviousYearPapers = papersListSM,
                Notifications = notificationsListSM,
                UserPerformance = userPerformance
            };

            return postDetailsSM;

        }

        public async Task<List<UserTestPerformanceSM>> GetUserPerformanceAsync(int postId, int userId)
        {
            var testDetailsDM = await _context.UserTestDetails
                .Where(x => x.PostId == postId && x.UserId == userId)
                .OrderByDescending(x => x.CreatedOnUtc)
                .ToListAsync();

            // Map to UserTestPerformanceSM using AutoMapper
            var performanceSM = _mapper.Map<List<UserTestPerformanceSM>>(testDetailsDM);

            return performanceSM;
        }

        #region GetNotificationsbyPostId
        public async Task<List<NotificationsSM>> GetNotificationsByPostIdAsync(int postId)
        {
            var notificationsDM = await _context.Notifications
                .Where(x => x.PostId == postId)
                .OrderByDescending(x => x.CreatedOnUtc)
                .ToListAsync();

            return _mapper.Map<List<NotificationsSM>>(notificationsDM);
        }

        #endregion

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

        public async Task AssignPreviousPapersAsync(List<PostPapersSM> models)
        {
            if (models == null || !models.Any())
                throw new AppException("No previous year papers provided.", HttpStatusCode.BadRequest);

            var postId = models.First().PostId;

            // 1️⃣ Validate Post
            var postExists = await _context.Posts.AnyAsync(p => p.Id == postId);
            if (!postExists)
                throw new AppException($"Post with Id {postId} does not exist.", HttpStatusCode.NotFound);

            var entitiesToAdd = new List<PostPapersDM>();

            foreach (var paper in models)
            {
                // 2️⃣ Validate each paper
                if (paper.PreviousYearPapersId <= 0)
                    throw new AppException("Invalid PreviousYearPaperId in one of the items.", HttpStatusCode.BadRequest);

                // ✅ Validate ExamYear is not in the future
                if (paper.ExamYear > DateTime.UtcNow)
                    throw new AppException("ExamYear cannot be in the future.", HttpStatusCode.BadRequest);

                // 3️⃣ Check if Previous Year Paper exists
                var paperExists = await _context.PreviousYearPapers
                    .AnyAsync(p => p.Id == paper.PreviousYearPapersId);
                if (!paperExists)
                    throw new AppException($"Previous year paper with Id {paper.PreviousYearPapersId} does not exist.", HttpStatusCode.NotFound);

                // 4️⃣ Check for duplicates
                var alreadyAssigned = await _context.PostPreviousYearPapers
                    .AnyAsync(pp => pp.PostId == paper.PostId && pp.PreviousYearPapersId == paper.PreviousYearPapersId);
                if (alreadyAssigned)
                    continue; // skip already assigned

                // 5️⃣ Map SM → DM using AutoMapper
                var entity = _mapper.Map<PostPapersDM>(paper);
                entitiesToAdd.Add(entity);
            }

            if (!entitiesToAdd.Any())
                throw new AppException("All provided papers were already assigned to this post.", HttpStatusCode.Conflict);

            // 6️⃣ Add all entities in batch
            _context.PostPreviousYearPapers.AddRange(entitiesToAdd);
            await _context.SaveChangesAsync();
        }

        public async Task AssignPostSyllabusAsync(List<PostSyllabusSM> models)
        {
            if (models == null || !models.Any())
                throw new AppException("No syllabus data provided.", HttpStatusCode.BadRequest);

            var postId = models.First().PostId;

            // 1️⃣ Validate Post
            var postExists = await _context.Posts.AnyAsync(p => p.Id == postId);
            if (!postExists)
                throw new AppException($"Post with Id {postId} does not exist.", HttpStatusCode.NotFound);

            var entitiesToAdd = new List<PostSyllabusDM>();

            foreach (var item in models)
            {
                // 2️⃣ Validate SyllabusId
                if (item.SyllabusId <= 0)
                    throw new AppException("Invalid SyllabusId in one of the items.", HttpStatusCode.BadRequest);

                
                var syllabusExists = await _context.Syllabus.AnyAsync(s => s.Id == item.SyllabusId);
                if (!syllabusExists)
                    throw new AppException($"Syllabus with Id {item.SyllabusId} does not exist.", HttpStatusCode.NotFound);

                // 5️⃣ Check for duplicate assignment
                var alreadyAssigned = await _context.PostSyllabus
                    .AnyAsync(ps => ps.PostId == item.PostId && ps.SyllabusId == item.SyllabusId && ps.YearOfExam == item.YearOfExam);
                if (alreadyAssigned)
                    continue; // Skip duplicates

                // 6️⃣ Map SM → DM
                var entity = _mapper.Map<PostSyllabusDM>(item);
                entitiesToAdd.Add(entity);
            }

            if (!entitiesToAdd.Any())
                throw new AppException("All provided syllabus entries were already assigned to this post.", HttpStatusCode.Conflict);

            // 7️⃣ Add all in batch
            _context.PostSyllabus.AddRange(entitiesToAdd);
            await _context.SaveChangesAsync();
        }


    }
}
