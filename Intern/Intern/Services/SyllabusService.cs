using System.Net;
using AutoMapper;
using Common.Helpers;
using Intern.Common.Helpers;
using Intern.Data;
using Intern.DataModels.Exams;
using Intern.ServiceModels.Exams;
using Microsoft.EntityFrameworkCore;

namespace Intern.Services
{
    public class SyllabusService
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;
        private readonly TokenHelper _tokenHelper;
        private readonly ImageHelper _imageHelper;

        public SyllabusService(ApiDbContext context,IMapper mapper,TokenHelper tokenHelper,ImageHelper imageHelper)
        {
            _context = context;
            _mapper = mapper;
            _tokenHelper = tokenHelper;
            _imageHelper = imageHelper;
        }

        public async Task<List<SyllabusSM>> GetSyllabusByPostIdAsync(int postId)
        {
            var syllabusDM = await _context.PostSyllabus
                .Include(ps => ps.Syllabus)
                .Where(ps => ps.PostId == postId)
                .Select(ps => ps.Syllabus)
                .ToListAsync();

            if (syllabusDM == null || !syllabusDM.Any())
                return new List<SyllabusSM>();

            var syllabusList = _mapper.Map<List<SyllabusSM>>(syllabusDM);

            foreach (var syllabus in syllabusList)
            {
                if (!string.IsNullOrEmpty(syllabus.FilePath) && File.Exists(syllabus.FilePath))
                {
                    syllabus.FilePath = _imageHelper.ConvertFileToBase64(syllabus.FilePath);
                }
            }

            return syllabusList;
        }


        // 🔹 Get All Syllabuses
        public async Task<IEnumerable<SyllabusSM>> GetAllAsync()
        {
            var list = await _context.Syllabus
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            var result = _mapper.Map<IEnumerable<SyllabusSM>>(list);

            // Convert file paths to base64
            foreach (var item in result)
            {
                if (!string.IsNullOrEmpty(item.FilePath) && File.Exists(item.FilePath))
                {
                    item.FilePath = _imageHelper.ConvertFileToBase64(item.FilePath);
                }
            }

            return result;
        }

        // 🔹 Get by ID
        public async Task<SyllabusSM> GetByIdAsync(int id)
        {
            var syllabus = await _context.Syllabus
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (syllabus == null)
                throw new AppException("Syllabus not found", HttpStatusCode.NotFound);

            var result = _mapper.Map<SyllabusSM>(syllabus);

            if (!string.IsNullOrEmpty(result.FilePath) && File.Exists(result.FilePath))
                result.FilePath = _imageHelper.ConvertFileToBase64(result.FilePath);

            return result;
        }

        // 🔹 Create Syllabus
        public async Task<string> CreateSyllabusAsync(SyllabusSM model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
                throw new AppException("Syllabus title is required", HttpStatusCode.BadRequest);

            var directory = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Syllabus");
            string filePath = null;

            if (!string.IsNullOrEmpty(model.FilePath))
            {
                filePath = await _imageHelper.SaveBase64FileAsync2(model.FilePath, directory, ".pdf");
            }

            var loginId = _tokenHelper.GetLoginIdFromToken();

            var entity = _mapper.Map<SyllabusDM>(model);
            entity.FilePath = filePath;
            entity.CreatedOnUtc = DateTime.UtcNow;
            entity.CreatedBy = loginId;

            _context.Syllabus.Add(entity);
            await _context.SaveChangesAsync();

            return "Syllabus inserted successfully";
        }

        // 🔹 Update Syllabus
        public async Task<SyllabusSM> UpdateSyllabusAsync(int syllabusId, SyllabusSM model)
        {
            var existing = await _context.Syllabus.FirstOrDefaultAsync(x => x.Id == syllabusId);

            if (existing == null)
                throw new AppException("Syllabus not found", HttpStatusCode.NotFound);

            var loginId = _tokenHelper.GetLoginIdFromToken();

            if (!string.IsNullOrWhiteSpace(model.Title))
                existing.Title = model.Title;

            if (!string.IsNullOrWhiteSpace(model.Description))
                existing.Description = model.Description;

            if (!string.IsNullOrEmpty(model.FilePath))
            {
                var directory = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Syllabus");
                string filePath = await _imageHelper.SaveBase64FileAsync2(model.FilePath, directory, ".pdf");
                existing.FilePath = filePath;
            }

            existing.LastModifiedOnUtc = DateTime.UtcNow;
            existing.LastModifiedBy = loginId;

            await _context.SaveChangesAsync();

            var response = _mapper.Map<SyllabusSM>(existing);

            if (!string.IsNullOrEmpty(existing.FilePath) && File.Exists(existing.FilePath))
            {
                response.FilePath = _imageHelper.ConvertFileToBase64(existing.FilePath);
            }

            return response;
        }

        // 🔹 Delete Syllabus
        public async Task<string> DeleteSyllabusAsync(int id)
        {
            var syllabus = await _context.Syllabus.FirstOrDefaultAsync(x => x.Id == id);

            if (syllabus == null)
                throw new AppException("Syllabus not found", HttpStatusCode.NotFound);

            if (!string.IsNullOrEmpty(syllabus.FilePath) && File.Exists(syllabus.FilePath))
                File.Delete(syllabus.FilePath);

            _context.Syllabus.Remove(syllabus);
            await _context.SaveChangesAsync();

            return "Syllabus deleted successfully";
        }
    }
}
