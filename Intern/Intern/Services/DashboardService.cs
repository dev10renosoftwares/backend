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
            var allPosts = await _deptService.GetPostsByDepartmentId(deptId, 0, 5);

            // 3. Collect PostIds from posts
            var postIds = allPosts.Select(p => p.Id).ToList();

            // 4. Fetch notifications for this department + posts
            var allNotifications = await _context.Notifications
                .Where(n => n.DepartmentId == deptId)      // Only filter by department
                .OrderByDescending(n => n.CreatedOnUtc)   // Most recent first
                .ToListAsync();                            // Get all notifications


            var notifications = _mapper.Map<List<NotificationsSM>>(allNotifications);

            // 6. Build dashboard response
            var dashboard = new DashboardSM
            {
                Exams = allPosts,

                UpcomingExams = allPosts
                    .Where(p => p.PostDate.HasValue && p.PostDate > DateTime.UtcNow)
                    .OrderBy(p => p.PostDate)
                    .Take(5)
                    .ToList(),

                RecommendedExams = allPosts
                    .OrderBy(p => p.PostDate)
                    .Take(5)
                    .ToList(),

                Notifications = notifications
            };

           
            return dashboard;
        }

    }
}
