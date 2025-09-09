using System.Net;
using AutoMapper;
using Common.Helpers;
using Intern.Data;
using Intern.DataModels.Exams;
using Intern.ServiceModels;
using Intern.ServiceModels.BaseServiceModels;
using Intern.ServiceModels.Exams;
using Intern.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Intern.Services
{
    public class DepartmentService
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;

        public DepartmentService(ApiDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<DepartmentSM>> GetAllAsync()
        {
            
            var entities = await _context.Departments.ToListAsync();

            // Map List<DepartmentDM> → List<DepartmentSM>
            return _mapper.Map<List<DepartmentSM>>(entities);
        }

        public async Task<DepartmentSM?> GetByIdAsync(int id)
        {
            var entity = await _context.Departments.FindAsync(id);

            if (entity == null)
                return null;

            return _mapper.Map<DepartmentSM>(entity); 
        }


        public async Task<string> CreateAsync(DepartmentSM department)
        {
            // Check if a department with the same name already exists
            bool exists = await _context.Departments
                .AnyAsync(d => d.DepartmentName.ToLower() == department.DepartmentName.Trim().ToLower());

            if (exists)
                throw new AppException("A department with the same name already exists.",HttpStatusCode.Conflict);

            // Map AddDepartmentSM to DepartmentDM
            var entity = _mapper.Map<DepartmentDM>(department);
            entity.CreatedOnUtc = DateTime.UtcNow;

            _context.Departments.Add(entity);
            await _context.SaveChangesAsync();

            return null; 
        }

        public async Task<bool> UpdateAsync(DepartmentSM updateDepartmentSM)
        {
           

            var existing = await _context.Departments.FindAsync(updateDepartmentSM.Id);
            if (existing == null) return false;

            if (!string.IsNullOrWhiteSpace(updateDepartmentSM.DepartmentName) && updateDepartmentSM.DepartmentName != "string")
                existing.DepartmentName = updateDepartmentSM.DepartmentName;

            if (!string.IsNullOrWhiteSpace(updateDepartmentSM.Description) && updateDepartmentSM.Description != "string")
                existing.Description = updateDepartmentSM.Description;


            existing.LastModifiedOnUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }




        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Department Id");

            var existing = await _context.Departments.FindAsync(id);
            if (existing == null) return false;

            _context.Departments.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignPostToDepartmentAsync(DepartmentPostsSM objSM)
        {
            var department = await _context.Departments.FindAsync(objSM.DepartmentId);
            if (department == null)
                throw new AppException($"Department with Id {objSM.DepartmentId} not found.", HttpStatusCode.NotFound);
            var post = await _context.Posts.FindAsync(objSM.PostId);
            if (post == null)
                throw new AppException($"Post with Id {objSM.PostId} not found.", HttpStatusCode.NotFound);

            var exists = await _context.DepartmentPosts
                .FirstOrDefaultAsync(dp => dp.DepartmentId == objSM.DepartmentId && dp.PostId == objSM.PostId);

            if (exists == null)
            {                
                var dm = _mapper.Map<DepartmentPostsDM>(objSM);
                await _context.DepartmentPosts.AddAsync(dm);
                if (await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }
                throw new AppException("Something went wrong while assigning post to department", HttpStatusCode.BadRequest);
            }
            else
            {
                throw new AppException("Posts is already assigned to this department ", HttpStatusCode.Conflict);
            }
            
        }
        public async Task<bool> RemovepostsfromDepartmentAsync(RemovepostsfromDepartmentSM removeposts)
        {
            var department = await _context.Departments.FindAsync(removeposts.DepartmentId);
            if (department == null)
                throw new AppException($"Department with Id {removeposts.DepartmentId} not found.", HttpStatusCode.NotFound);

            var post = await _context.Posts.FindAsync(removeposts.PostId);
            if (post == null)
                throw new AppException($"Post with Id {removeposts.PostId} not found.", HttpStatusCode.NotFound);

           
            var deptPost = await _context.DepartmentPosts
                .FirstOrDefaultAsync(dp => dp.DepartmentId == removeposts.DepartmentId && dp.PostId == removeposts.PostId);

            if (deptPost == null)
                throw new AppException("This post is not assigned to the department.", HttpStatusCode.BadRequest);

            _context.DepartmentPosts.Remove(deptPost);
            await _context.SaveChangesAsync();

            return true;

        }

      


    }
}
