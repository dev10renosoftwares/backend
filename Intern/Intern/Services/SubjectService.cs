using AutoMapper;
using Common.Helpers;
using Google;
using Intern.Common.Helpers;
using Intern.Data;
using Intern.DataModels.Exams;
using Intern.ServiceModels;
using Intern.ServiceModels.Exams;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Intern.Services
{
    public class SubjectService
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;
        private readonly TokenHelper _tokenHelper;

        public SubjectService(ApiDbContext context, IMapper mapper,TokenHelper tokenHelper)
        {
            _context = context;
            _mapper = mapper;
            _tokenHelper = tokenHelper;
        }
        public async Task<IEnumerable<SubjectSM>> GetAllAsync()
        {
            var entities = await _context.Subjects.ToListAsync();
            return _mapper.Map<List<SubjectSM>>(entities);
        }

        public async Task<SubjectSM?> GetByIdAsync(int id)
        {
            var entity = await _context.Subjects.FindAsync(id);
            return entity == null ? null : _mapper.Map<SubjectSM>(entity);
        }

        public async Task<string> CreateAsync(SubjectSM subject)
        {
            bool exists = await _context.Subjects
                .AnyAsync(s => s.SubjectName.ToLower() == subject.SubjectName.Trim().ToLower());

            if (exists)
                throw new AppException("A subject with the same name already exists.", HttpStatusCode.Conflict);

            var LoginId = _tokenHelper.GetLoginIdFromToken();

            var entity = _mapper.Map<SubjectDM>(subject);
            entity.CreatedOnUtc = DateTime.UtcNow;
            entity.CreatedBy = LoginId;

            _context.Subjects.Add(entity);
            await _context.SaveChangesAsync();
            return "Subject created successfully";
        }

        public async Task<string> UpdateAsync(int id, SubjectSM model)
        {
            if (id <= 0)
                throw new AppException("Invalid Subject Id", HttpStatusCode.BadRequest);

            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
                throw new AppException("Subject not found", HttpStatusCode.NotFound);

            // Map incoming model values into the existing entity
            _mapper.Map(model, subject);

            // Update metadata
            var loginId = _tokenHelper.GetLoginIdFromToken();
            subject.LastModifiedOnUtc = DateTime.UtcNow;
            subject.LastModifiedBy = loginId;

            await _context.SaveChangesAsync();

            return "Subject updated successfully";
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new AppException("Invalid Subject Id", HttpStatusCode.BadRequest);

            var existing = await _context.Subjects.FindAsync(id);
            if (existing == null) return false;

            _context.Subjects.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> CreateAndAssignSubjectAsync(AddSubjectandAssignSM request)
        {
            
            var post = await _context.Posts.FindAsync(request.PostId);
            if (post == null)
                throw new AppException($"Post with Id {request.PostId} not found.", HttpStatusCode.NotFound);

            bool exists = await _context.Subjects
                .AnyAsync(s => s.SubjectName.ToLower() == request.SubjectName.Trim().ToLower());

            if (exists)
                throw new AppException("A subject with the same name already exists.", HttpStatusCode.Conflict);

            var loginId = _tokenHelper.GetLoginIdFromToken();
            // Step 3: Map to SubjectDM
            var subjectEntity = _mapper.Map<SubjectDM>(request);
            subjectEntity.CreatedOnUtc = DateTime.UtcNow;
            subjectEntity.CreatedBy = loginId;

            _context.Subjects.Add(subjectEntity);
            await _context.SaveChangesAsync();

            // Step 4: Map to PostSubjectsDM (join table)
            var postSubjectEntity = new SubjectPostDM
            {
                PostId = request.PostId,
                SubjectId = subjectEntity.Id,
                
            };

            _context.SubjectPosts.Add(postSubjectEntity);
            await _context.SaveChangesAsync();

            return "Subject created and assigned successfully.";
        }

        public async Task<bool> RemoveSubjectsFromPostAsync(int subjectPostId)
        {
            if (subjectPostId <= 0)
                throw new AppException("Invalid SubjectPost Id", HttpStatusCode.BadRequest);

            var subjectPost = await _context.SubjectPosts.FindAsync(subjectPostId);
            if (subjectPost == null)
                throw new AppException($"SubjectPost with Id {subjectPostId} not found.", HttpStatusCode.NotFound);

            _context.SubjectPosts.Remove(subjectPost);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<PostSubjectsResponseSM> GetSubjectsByPostId(int postId)
        {
            var existingPost = await _context.Posts.FindAsync(postId);
            if (existingPost == null)
            {
                throw new AppException("PostId Not Found", HttpStatusCode.NotFound);
            }

            var response = new PostSubjectsResponseSM()
            {
                Post = _mapper.Map<PostSM>(existingPost),
                Subjects = new List<PostSubjectRelationSM>()
            };

            var postSubjects = await _context.SubjectPosts
                .Where(x => x.PostId == postId)
                .ToListAsync();

            if (postSubjects.Count == 0)
            {
                return response; // return empty if no subjects
            }

            var subjects = new List<PostSubjectRelationSM>();
            foreach (var subjectPost in postSubjects)
            {
                var subjectEntity = await _context.Subjects.FindAsync(subjectPost.SubjectId);
                if (subjectEntity != null)
                {
                    var subjectSm = _mapper.Map<SubjectSM>(subjectEntity);

                    var relation = new PostSubjectRelationSM()
                    {
                        SubjectPostId = subjectPost.Id,
                        Subject = subjectSm
                    };

                    subjects.Add(relation);
                }
            }

            response.Subjects = subjects;
            return response;
        }



    }

}
