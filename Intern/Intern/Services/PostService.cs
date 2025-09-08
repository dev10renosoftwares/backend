using AutoMapper;
using Common.Helpers;
using Intern.Data;
using Intern.DataModels.Exams;
using Intern.ServiceModels;
using System.Net;
using Microsoft.EntityFrameworkCore;

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

        public async Task<string> CreateAsync(AddPostSM post)
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

        public async Task<bool> UpdateAsync(int id, AddPostSM updatePostSM)
        {
            if (id <= 0)
                throw new AppException("Invalid Post Id", HttpStatusCode.BadRequest);

            var existing = await _context.Posts.FindAsync(id);
            if (existing == null) return false;

            if (!string.IsNullOrWhiteSpace(updatePostSM.PostName) && updatePostSM.PostName != "string")
                existing.PostName = updatePostSM.PostName;

            if (!string.IsNullOrWhiteSpace(updatePostSM.Description) && updatePostSM.Description != "string")
                existing.Description = updatePostSM.Description;

            existing.LastModifiedOnUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Post Id");

            var existing = await _context.Posts.FindAsync(id);
            if (existing == null) return false;

            _context.Posts.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
