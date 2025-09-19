using System.Net;
using AutoMapper;
using Common.Helpers;
using Intern.Data;
using Intern.DataModels.Enums;
using Intern.DataModels.Exams;
using Intern.ServiceModels;
using Intern.ServiceModels.Enums;
using Intern.ServiceModels.Exams;
using Microsoft.EntityFrameworkCore;

namespace Intern.Services
{
    public class MCQService
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;
        private readonly PostService _postSevice;        

        public MCQService(ApiDbContext context,IMapper mapper, PostService postService)
        {
            _context = context;
            _mapper = mapper;
            _postSevice = postService;
        }

        public async Task<List<MCQsSM>> GetAllAsync()
        {
            var mcqs = await _context.MCQs.ToListAsync();
            return _mapper.Map<List<MCQsSM>>(mcqs);
        }

        public async Task<MCQsSM?> GetByIdAsync(int id)
        {
            var entity = await _context.MCQs.FindAsync(id);

            if (entity == null)
                return null;

            return _mapper.Map<MCQsSM>(entity);
        }

        public async Task<MCQsSM> CreateAsync(MCQsSM addmcqs)
        {
           
            var exists = await _context.MCQs
                .FirstOrDefaultAsync(mcq =>mcq.Question == addmcqs.Question);

            if (exists != null)
            {
                return _mapper.Map<MCQsSM>(exists);
            }

           
            var entity = _mapper.Map<MCQsDM>(addmcqs);
            entity.CreatedOnUtc = DateTime.UtcNow;

            await _context.MCQs.AddAsync(entity);
            if(await _context.SaveChangesAsync() > 0)
            {
                return await GetByIdAsync(entity.Id);
            }

            return null;
        }

        public async Task<string> AddMcqsToPost(int postId, List<MCQsSM> objSM)
        {
            var existingPost = await _postSevice.GetByIdAsync(postId);
            if (existingPost == null)
            {
                throw new AppException("Cannot add MCQ as Post Id is not found");
            }

            if (objSM == null || objSM.Count == 0)
            {
                throw new AppException("MCQs not found");
            }

            // ✅ Begin transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var sm in objSM)
                {
                    sm.CreatedBy = "null user";
                    var added = await CreateAsync(sm);  
                    if (added != null)
                    {
                        var existingRelation = await _context.MCQPostSubjects.Where(x => x.MCQId == added.Id && x.PostId == postId).FirstOrDefaultAsync();
                        if(existingRelation != null)
                        {
                            continue;
                        }
                        var mcqPost = new MCQPostSubjectDM
                        {
                            MCQId = added.Id,
                            PostId = postId,
                            MCQType = McqTypeDM.Post
                        };

                        await _context.MCQPostSubjects.AddAsync(mcqPost);
                        await _context.SaveChangesAsync();
                    }
                }

                // ✅ Commit transaction
                await transaction.CommitAsync();
                return "true";
            }
            catch (Exception e)
            {
                // ❌ Rollback transaction
                await transaction.RollbackAsync();
                throw e;
            }
        }


        public async Task<string> UpdateAsync(MCQsSM updatedMcq)
        {
            var entity = await _context.MCQs.FindAsync(updatedMcq.Id);

            if (entity == null)
                throw new AppException($"MCQ with Id {updatedMcq.Id} not found.", HttpStatusCode.NotFound);


            if (!string.IsNullOrEmpty(updatedMcq.Question))
                entity.Question = updatedMcq.Question;

            if (!string.IsNullOrEmpty(updatedMcq.OptionA))
                entity.OptionA = updatedMcq.OptionA;

            if (!string.IsNullOrEmpty(updatedMcq.OptionB))
                entity.OptionB = updatedMcq.OptionB;

            if (!string.IsNullOrEmpty(updatedMcq.OptionC))
                entity.OptionC = updatedMcq.OptionC;

            if (!string.IsNullOrEmpty(updatedMcq.OptionD))
                entity.OptionD = updatedMcq.OptionD;

            if (!string.IsNullOrEmpty(updatedMcq.Answer))
                entity.Answer = updatedMcq.Answer;

            if (!string.IsNullOrEmpty(updatedMcq.Explanation))
                entity.Explanation = updatedMcq.Explanation;

            _mapper.Map(updatedMcq, entity);

           


            entity.LastModifiedOnUtc = DateTime.UtcNow;
           

            await _context.SaveChangesAsync();

            return "MCQ updated successfully";
        }



        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new AppException("Invalid MCQ Id", HttpStatusCode.BadRequest);

            var existing = await _context.MCQs.FindAsync(id);
            if (existing == null) return false;

            _context.MCQs.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }



        public async Task<bool> AssignMCQToSubjectOrPostAsync(MCQSubjectPostSM objSM)
        {
            // Validate required field
            if (objSM.MCQId <= 0)
                throw new AppException("MCQId is required", HttpStatusCode.BadRequest);

            // Determine McqType based on SubjectId and PostId
            if (objSM.SubjectId.HasValue && objSM.PostId.HasValue)
            {
                objSM.McqType = McqTypeSM.General;
            }
            else if (objSM.PostId.HasValue && !objSM.SubjectId.HasValue)
            {
                objSM.McqType = McqTypeSM.Post;
            }
            else if (objSM.SubjectId.HasValue && !objSM.PostId.HasValue)
            {
                objSM.McqType = McqTypeSM.Subject;
            }
            else
            {
                throw new AppException("Either SubjectId, PostId or both must be provided", HttpStatusCode.BadRequest);
            }

            // Validate subject if given
            if (objSM.SubjectId.HasValue)
            {
                var subject = await _context.Subjects.FindAsync(objSM.SubjectId.Value);
                if (subject == null)
                    throw new AppException($"Subject with Id {objSM.SubjectId.Value} not found", HttpStatusCode.NotFound);
            }

            // Validate post if given
            if (objSM.PostId.HasValue)
            {
                var post = await _context.Posts.FindAsync(objSM.PostId.Value);
                if (post == null)
                    throw new AppException($"Post with Id {objSM.PostId.Value} not found", HttpStatusCode.NotFound);
            }

            // Validate MCQ existence
            var mcq = await _context.MCQs.FindAsync(objSM.MCQId);
            if (mcq == null)
                throw new AppException($"MCQ with Id {objSM.MCQId} not found", HttpStatusCode.NotFound);

            // Prevent duplicates
            var exists = await _context.MCQPostSubjects
                .FirstOrDefaultAsync(x =>
                    x.MCQId == objSM.MCQId &&
                    x.SubjectId == objSM.SubjectId &&
                    x.PostId == objSM.PostId &&
                    x.MCQType == (McqTypeDM)objSM.McqType);

            if (exists != null)
                throw new AppException("MCQ is already assigned", HttpStatusCode.Conflict);

            // Map and save
            var dm = _mapper.Map<MCQPostSubjectDM>(objSM);
            await _context.MCQPostSubjects.AddAsync(dm);

            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }

            throw new AppException("Something went wrong while assigning MCQ", HttpStatusCode.BadRequest);
        }

    }
}
