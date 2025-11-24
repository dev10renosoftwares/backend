using System.Net;
using AutoMapper;
using Common.Helpers;
using Intern.Data;
using Intern.DataModels.Exams;
using Intern.ServiceModels;
using Intern.ServiceModels.Exams;
using Intern.ServiceModels.User;
using Microsoft.EntityFrameworkCore;

namespace Intern.Services
{
    public class DashboardService
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;
        private readonly PostService _postService;
        private readonly DepartmentService _deptService;

        public DashboardService(ApiDbContext context,IMapper mapper,PostService postService, DepartmentService deptService)
        {
           _context = context;
            _mapper = mapper;
            _postService = postService;
            _deptService = deptService;
        }

        public async Task<DashboardSM> GetDashboardAsync(int deptId)
        {
            // 1. Validate department
            var existingDept = await _context.Departments.FindAsync(deptId);
            if (existingDept == null)
            {
                throw new AppException("DepartmentId Not Found", HttpStatusCode.NotFound);
            }

            // 2. Fetch all posts for this department
            var allPosts = await _deptService.GetPostsByDepartmentId(deptId, 0, 20);

            // 3. Collect PostIds from posts
            var postIds = allPosts.Select(p => p.Id).ToList();

            // 4. Fetch notifications for this department + posts
              var allNotifications = await _context.Notifications
                .Where(n => n.DepartmentId == deptId)      // Only filter by department
                .OrderByDescending(n => n.CreatedOnUtc)   // Most recent first
                .ToListAsync();                            // Get all notifications


            var notifications = _mapper.Map<List<NotificationsSM>>(allNotifications);

            // Fetch recommended exams
            var recommendedExams = await GetRecommendedExamsAsync(deptId);

            // 6. Build dashboard response
            var dashboard = new DashboardSM
            {
                Exams = allPosts,

                UpcomingExams = allPosts
                    .Where(p => p.PostDate.HasValue && p.PostDate > DateTime.UtcNow)
                    .OrderBy(p => p.PostDate)
                    .Take(5)
                    .ToList(),

                //RecommendedExams = allPosts
                //    .OrderBy(p => p.PostName)
                //    .ToList(),

                RecommendedExams = recommendedExams,




                Notifications = notifications
            };

           
            return dashboard;
        }


        public async Task<List<PostSM>> GetRecommendedExamsAsync(int deptId)
        {
            // 1️⃣ Validate Department
            var existingDept = await _context.Departments.FindAsync(deptId);
            if (existingDept == null)
                throw new AppException("DepartmentId Not Found", HttpStatusCode.NotFound);

            // 2️⃣ Fetch all DepartmentPosts + Post details
            var posts = await (from dp in _context.DepartmentPosts
                               join p in _context.Posts on dp.PostId equals p.Id
                               where dp.DepartmentId == deptId
                               select new
                               {
                                   p.Id,
                                   p.PostName,
                                   p.Description,
                                   dp.NotificationNumber,
                                   dp.PostDate
                               })  
                               .ToListAsync();

            if (!posts.Any())
                return new List<PostSM>();

            // 3️⃣ Group by PostId only (to make each post appear once)
            var grouped = posts
                .GroupBy(x => x.Id)
                .Select(g => new
                {
                    PostId = g.Key,
                    PostName = g.Select(x => x.PostName).FirstOrDefault(),
                    Description = g.Select(x => x.Description).FirstOrDefault(),
                    NotificationNumber = g.Select(x => x.NotificationNumber)
                                           .Where(x => !string.IsNullOrEmpty(x))
                                           .FirstOrDefault(),
                    Frequency = g.Count(),                    // how many times this post appeared
                    LatestPostDate = g.Max(x => x.PostDate)   // the most recent PostDate
                })
                .ToList();

            var today = DateTime.UtcNow;

            // 4️⃣ Separate upcoming and past for proper sorting
            var upcoming = grouped
                .Where(p => p.LatestPostDate >= today)
                .OrderByDescending(p => p.Frequency)
                .ThenBy(p => p.LatestPostDate)
                .ThenBy(p => p.PostName)
                .ToList();

            var past = grouped
                .Where(p => p.LatestPostDate < today)
                .OrderByDescending(p => p.Frequency)
                .ThenByDescending(p => p.LatestPostDate)
                .ThenBy(p => p.PostName)
                .ToList();

            // 5️⃣ Merge upcoming and past, now only one record per post
            var finalList = upcoming
                .Concat(past)
                .Select(p => new PostSM
                {
                    Id = p.PostId,
                    PostName = p.PostName,
                    Description = p.Description,
                    NotificationNumber = p.NotificationNumber,
                    PostDate = p.LatestPostDate
                })
                .Distinct()
                .ToList();

            return finalList;
        }


        public async Task<List<PostSM>> GetTopRecommendedExamsAsync(int deptId, int skip, int top)
        {
            // 1️⃣ Validate Department
            var existingDept = await _context.Departments.FindAsync(deptId);
            if (existingDept == null)
                throw new AppException("DepartmentId Not Found", HttpStatusCode.NotFound);

            var today = DateTime.UtcNow.Date;

            // 2️⃣ Single query: group, count frequency, sort
            var recommendedPosts = await _context.DepartmentPosts
                .Where(dp => dp.DepartmentId == deptId && dp.PostId != null)
                .GroupBy(dp => new
                {
                    dp.PostId,
                    dp.Post.PostName,
                    dp.Post.Description
                })
                .Select(g => new
                {
                    PostId = g.Key.PostId.Value,
                    PostName = g.Key.PostName,
                    Description = g.Key.Description,
                    NotificationNumber = g.Select(x => x.NotificationNumber)
                                          .Where(x => !string.IsNullOrEmpty(x))
                                          .FirstOrDefault(),
                    Frequency = g.Count(),
                    LatestPostDate = g.Max(x => x.PostDate)
                })
                .OrderByDescending(p => p.Frequency)                             // Most frequent first
                .ThenBy(p => p.LatestPostDate.Date >= today ? p.LatestPostDate : DateTime.MaxValue) // Upcoming nearest
                .ThenByDescending(p => p.LatestPostDate.Date < today ? p.LatestPostDate : DateTime.MinValue) // Past recent first
                .ThenBy(p => p.PostName)                                         // Tie-breaker
                .Skip(skip)
                .Take(top)
                .Select(p => new PostSM
                {
                    Id = p.PostId,
                    PostName = p.PostName,
                    Description = p.Description,
                    NotificationNumber = p.NotificationNumber,
                    PostDate = p.LatestPostDate
                })
                .ToListAsync();

            return recommendedPosts;
        }

        public async Task<List<PostSM>> GetTopUpcomingExamsAsync(int deptId, int skip, int top)
        {
            // 1️⃣ Validate Department
            var existingDept = await _context.Departments.FindAsync(deptId);
            if (existingDept == null)
                throw new AppException("DepartmentId Not Found", HttpStatusCode.NotFound);

            // 2️⃣ Fetch Upcoming Exams for the Department
            var upcomingExams = await _context.DepartmentPosts
                .Where(dp => dp.DepartmentId == deptId
                             && dp.PostDate > DateTime.UtcNow) 
                .OrderBy(dp => dp.PostDate)  
                .Skip(skip)
                .Take(top)
                .Select(dp => new PostSM
                {
                    Id = dp.Post.Id,
                    PostName = dp.Post.PostName,
                    Description = dp.Post.Description,
                    PostDate = dp.PostDate,
                    NotificationNumber = dp.NotificationNumber
                })
                .ToListAsync();

            return upcomingExams;
        }



    }

}

