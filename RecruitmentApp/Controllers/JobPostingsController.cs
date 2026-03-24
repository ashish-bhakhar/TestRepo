using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Data;
using RecruitmentApp.Models;

namespace RecruitmentApp.Controllers
{
    public class JobPostingsController : Controller
    {
        private readonly RecruitmentDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public JobPostingsController(RecruitmentDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: JobPostings
        public async Task<IActionResult> Index(string? status, string? search)
        {
            var query = _context.JobPostings.Include(j => j.Applications).AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(j => j.Status == status);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(j => j.Title.Contains(search) || j.Department.Contains(search) || j.Location.Contains(search));

            ViewBag.CurrentStatus = status;
            ViewBag.CurrentSearch = search;
            return View(await query.OrderByDescending(j => j.PostedDate).ToListAsync());
        }

        // GET: JobPostings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var job = await _context.JobPostings
                .Include(j => j.Applications)
                    .ThenInclude(a => a.Candidate)
                .Include(j => j.Resumes)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null) return NotFound();
            return View(job);
        }

        // GET: JobPostings/Create
        public IActionResult Create()
        {
            return View(new JobPosting());
        }

        // POST: JobPostings/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobPosting job)
        {
            if (ModelState.IsValid)
            {
                job.PostedDate = DateTime.Now;
                _context.Add(job);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Job posting created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(job);
        }

        // GET: JobPostings/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var job = await _context.JobPostings.FindAsync(id);
            if (job == null) return NotFound();
            return View(job);
        }

        // POST: JobPostings/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JobPosting job)
        {
            if (id != job.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(job);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Job posting updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(job);
        }

        // POST: JobPostings/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var job = await _context.JobPostings.FindAsync(id);
            if (job != null)
            {
                _context.JobPostings.Remove(job);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Job posting deleted.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: JobPostings/UploadResumes/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadResumes(int id, List<IFormFile> resumes, string? candidateName, string? candidateEmail)
        {
            var job = await _context.JobPostings.FindAsync(id);
            if (job == null) return NotFound();

            if (resumes == null || !resumes.Any())
            {
                TempData["Error"] = "Please select at least one resume file.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Validate file types
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
            var uploadedFiles = 0;

            foreach (var file in resumes)
            {
                if (file.Length > 0)
                {
                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(extension))
                    {
                        TempData["Error"] = $"File {file.FileName} is not allowed. Only PDF, DOC, and DOCX files are accepted.";
                        continue;
                    }

                    if (file.Length > 10 * 1024 * 1024) // 10MB limit
                    {
                        TempData["Error"] = $"File {file.FileName} is too large. Maximum size is 10MB.";
                        continue;
                    }

                    // Create uploads directory if it doesn't exist
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "resumes");
                    Directory.CreateDirectory(uploadsFolder);

                    // Generate unique filename
                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Save to database
                    var resume = new Resume
                    {
                        JobPostingId = id,
                        FileName = file.FileName,
                        FilePath = $"/uploads/resumes/{uniqueFileName}",
                        FileSize = file.Length,
                        UploadedDate = DateTime.Now,
                        CandidateName = candidateName,
                        CandidateEmail = candidateEmail
                    };

                    _context.Resumes.Add(resume);
                    uploadedFiles++;
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"{uploadedFiles} resume(s) uploaded successfully!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: JobPostings/DownloadResume/5
        public async Task<IActionResult> DownloadResume(int id)
        {
            var resume = await _context.Resumes.FindAsync(id);
            if (resume == null) return NotFound();

            var filePath = Path.Combine(_environment.WebRootPath, resume.FilePath.TrimStart('/'));
            
            if (!System.IO.File.Exists(filePath))
            {
                TempData["Error"] = "File not found.";
                return RedirectToAction(nameof(Details), new { id = resume.JobPostingId });
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, "application/octet-stream", resume.FileName);
        }

        // POST: JobPostings/DeleteResume/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteResume(int id)
        {
            var resume = await _context.Resumes.FindAsync(id);
            if (resume == null) return NotFound();

            var jobId = resume.JobPostingId;

            // Delete physical file
            var filePath = Path.Combine(_environment.WebRootPath, resume.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Delete from database
            _context.Resumes.Remove(resume);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Resume deleted.";
            return RedirectToAction(nameof(Details), new { id = jobId });
        }
    }
}
