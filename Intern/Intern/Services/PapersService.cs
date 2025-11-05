using System.Net;
using AutoMapper;
using Common.Helpers;
using Google.Apis.Drive.v3;
using Intern.Common.Helpers;
using Intern.Data;
using Intern.DataModels.Exams;
using Intern.ServiceModels.Exams;
using Microsoft.EntityFrameworkCore;

namespace Intern.Services
{
    public class PapersService
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;
        private readonly ImageHelper _imageHelper;
        private readonly TokenHelper _tokenHelper;
        private readonly GoogleDriveService _googleDriveService;

        public PapersService(ApiDbContext context,IMapper mapper,ImageHelper imageHelper,TokenHelper tokenHelper,GoogleDriveService googleDriveService)
        {
            _context = context;
            _mapper = mapper;
            _imageHelper = imageHelper;
            _tokenHelper = tokenHelper;
            _googleDriveService = googleDriveService;
        }

        public async Task<List<PapersSM>> GetPapersByPostIdAsync(int postId)
        {
            var paperIds = await _context.PostPreviousYearPapers
                .Where(x => x.PostId == postId)
                .Select(x => x.PreviousYearPapersId)
                .ToListAsync();

            var papers = await _context.PreviousYearPapers
                .Where(p => paperIds.Contains(p.Id))
                .ToListAsync();

            if (papers == null || !papers.Any())
                return new List<PapersSM>();

            var papersList = _mapper.Map<List<PapersSM>>(papers);

            foreach (var paper in papersList)
            {
                if (!string.IsNullOrEmpty(paper.FilePath) && File.Exists(paper.FilePath))
                {
                    paper.FilePath = _imageHelper.ConvertFileToBase64(paper.FilePath);
                }
            }

            return papersList;
        }


        public async Task<IEnumerable<PapersSM>> GetAllAsync()
        {
            var papers = await _context.PreviousYearPapers
                .AsNoTracking()
                .ToListAsync();

            var result = _mapper.Map<IEnumerable<PapersSM>>(papers);

            // Convert file to Base64 (if exists)
            foreach (var item in result)
            {
                if (!string.IsNullOrEmpty(item.FilePath) && File.Exists(item.FilePath))
                    item.FilePath = _imageHelper.ConvertFileToBase64(item.FilePath);
                else
                    item.FilePath = null;
            }

            return result;
        }

        // 🔹 Get paper by ID
        public async Task<PapersSM> GetByIdAsync(int id)
        {
            var paper = await _context.PreviousYearPapers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (paper == null)
                throw new AppException("Paper not found", HttpStatusCode.NotFound);

            var result = _mapper.Map<PapersSM>(paper);

            if (!string.IsNullOrEmpty(result.FilePath) && File.Exists(result.FilePath))
                result.FilePath = _imageHelper.ConvertFileToBase64(result.FilePath);

            return result;
        }

        // 🔹 Create paper
        public async Task<string> CreatePaperAsync(PapersSM model)
        {
            if (string.IsNullOrWhiteSpace(model.PaperTitle))
                throw new AppException("Paper title is required", HttpStatusCode.BadRequest);

            var directory = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Papers");

            string filePath = null;
            if (!string.IsNullOrEmpty(model.FilePath))
            {
                filePath = await _imageHelper.SaveBase64FileAsync2(model.FilePath, directory, ".pdf");
            }
            var loginid = _tokenHelper.GetLoginIdFromToken();

            var entity = _mapper.Map<PapersDM>(model);
            entity.FilePath = filePath;
            entity.CreatedOnUtc = DateTime.UtcNow;
            entity.CreatedBy = loginid;

            _context.PreviousYearPapers.Add(entity);
            await _context.SaveChangesAsync();

            return "paper Inserted successfully";
        }

        // 🔹 Update paper
        public async Task<PapersSM> UpdatePaperAsync(int paperId, PapersSM model)
        {
            var existing = await _context.PreviousYearPapers
                .FirstOrDefaultAsync(x => x.Id == paperId);

            if (existing == null)
                throw new AppException("Paper not found", HttpStatusCode.NotFound);

            var loginId = _tokenHelper.GetLoginIdFromToken();

            // ✅ Update only provided fields
            if (!string.IsNullOrWhiteSpace(model.PaperTitle))
                existing.PaperTitle = model.PaperTitle;

            if (!string.IsNullOrWhiteSpace(model.Description))
                existing.Description = model.Description;

            // ✅ File update logic (save new if provided, else retain old)
            if (!string.IsNullOrEmpty(model.FilePath))
            {
                var directory = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Papers");
                string filePath = await _imageHelper.SaveBase64FileAsync2(model.FilePath, directory, ".pdf");
                existing.FilePath = filePath;
            }
            else
            {
                // Keep the existing file if nothing new was sent
                model.FilePath = existing.FilePath;
            }

            // ✅ Update metadata
            existing.LastModifiedOnUtc = DateTime.UtcNow;
            existing.LastModifiedBy = loginId;

            await _context.SaveChangesAsync();

            // ✅ Map updated entity back to SM
            var response = _mapper.Map<PapersSM>(existing);

            // ✅ Convert file path to Base64 for response (like your image logic)
            if (!string.IsNullOrEmpty(existing.FilePath) && File.Exists(existing.FilePath))
            {
                response.FilePath = _imageHelper.ConvertFileToBase64(existing.FilePath);
            }

            return response;
        }





        // 🔹 Delete paper
        public async Task<string> DeletePaperAsync(int id)
        {
            var paper = await _context.PreviousYearPapers.FirstOrDefaultAsync(x => x.Id == id);

            if (paper == null)
                throw new AppException("Paper not found", HttpStatusCode.NotFound);

            // Delete file if exists
            if (!string.IsNullOrEmpty(paper.FilePath) && File.Exists(paper.FilePath))
                File.Delete(paper.FilePath);

            _context.PreviousYearPapers.Remove(paper);
            await _context.SaveChangesAsync();

            return "Paper deleted successfully";
        }
    }
}
