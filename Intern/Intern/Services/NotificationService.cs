using System.Net;
using AutoMapper;
using Common.Helpers;
using Intern.Data;
using Intern.DataModels.Enums;
using Intern.DataModels.Exams;
using Intern.ServiceModels.Exams;
using Microsoft.EntityFrameworkCore;

namespace Intern.Services
{
    public class NotificationService
    {
        private readonly ApiDbContext _context;
        private readonly PostService _postService;
        private readonly DepartmentService _deptService;
        private readonly IMapper _mapper;

        public NotificationService(ApiDbContext context, IMapper mapper, PostService postService, DepartmentService deptService)
        {
            _context = context;
            _mapper = mapper;
            _postService = postService;
            _deptService = deptService;
        }

        #region GET ALL

        #region ALL Notifications
        public async Task<List<NotificationsSM>> GetAllNotifications(int skip, int top)
        {
            var dms = await _context.Notifications
                .Skip(skip)
                .Take(top)
                .ToListAsync();
            if(dms.Count == 0)
            {
                return new List<NotificationsSM>();

            }
            return _mapper.Map<List<NotificationsSM>>(dms);
        }

        #region Count
        public async Task<int> GetAllNotificationsCount()
        {
            var count = await _context.Notifications.CountAsync();
            return count;
        }
        #endregion Count

        #endregion ALL Notifications

       

        #region Get By Post and DepartmentId

        #region ALL notifications of Department post
        public async Task<List<NotificationsSM>> GetAllNotificationsOfDeptPost(int deptId, int postId,int skip, int top)
        {
            var dms = await _context.Notifications
                .Where(x=>x.DepartmentId == deptId && x.PostId == postId)
                .Skip(skip)
                .Take(top)
                .ToListAsync();
            if (dms.Count == 0)
            {
                return new List<NotificationsSM>();

            }
            return _mapper.Map<List<NotificationsSM>>(dms);
        }

        #endregion ALL notifications of Department post

        #region Count
        public async Task<int> GetAllNotificationsOdDeptPostCount(int deptId, int postId)
        {
            var count = await _context.Notifications
                .Where(x => x.DepartmentId == deptId && x.PostId == postId)
                .Select(x=>x.Id)
                .CountAsync();
            return count;
        }
        #endregion Count

        #endregion Get By Post and DepartmentId

        #endregion GET ALL

        #region Get Single

        public async Task<NotificationsSM> GetNotificationById(int id)
        {
            var dm = await _context.Notifications.FindAsync(id);
            if(dm == null)
            {
                throw new AppException("DepartmentId Not Found", HttpStatusCode.NotFound);
            }
            return _mapper.Map<NotificationsSM>(dm);

        }

        #endregion Get Single

        #region Update

        public async Task<NotificationsSM> UpdateById(int id, NotificationsSM objSM)
        {
            var dm = await _context.Notifications.FindAsync(id);
            if(dm == null)
            {
                return null;
            }
            dm.Title = objSM.Title;
            dm.Message = objSM.Message;
            dm.NotificationType = (NotificationTypeDM)objSM.NotificationType;
            if(await _context.SaveChangesAsync() > 0)
            {
                return _mapper.Map<NotificationsSM>(dm);
            }
            return null;
        }

        #endregion Update

        #region Create

        public async Task<NotificationsSM> PostNotification(NotificationsSM objSM)
        {
            if(objSM == null)
            {
                return null;
            }
            var existingPost = await _postService.GetByIdAsync(objSM.PostId);
            if(existingPost == null)
            {
                return null;
            }
            var existingDepartment = await _deptService.GetByIdAsync(objSM.DepartmentId);
            if (existingDepartment == null)
            {
                return null;
            }

            var dm = _mapper.Map<NotificationsDM>(objSM);
            dm.CreatedBy = "null user";
            await _context.Notifications.AddAsync(dm);
            if(await _context.SaveChangesAsync() > 0)
            {
                return await GetNotificationById(dm.Id);
            }
            return null;
        }

        #endregion Create

        #region Delete

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new AppException("Invalid NotificationId", HttpStatusCode.BadRequest);

            var existing = await _context.Notifications.FindAsync(id);
            if (existing == null) return false;

            _context.Notifications.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion Delete
    }
}
