using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Data;
using RecruitmentApp.Models;

namespace RecruitmentApp.Controllers
{
    public class CandidatesController : Controller
    {
        private readonly RecruitmentDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CandidatesController(RecruitmentDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Candidates
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Candidates.Include(c => c.Applications).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.FirstName.Contains(search) ||
                                         c.LastName.Contains(search) ||
                                         c.Email.Contains(search) ||
                                         (c.CurrentTitle != null && c.CurrentTitle.Contains(search)));

            ViewBag.CurrentSearch = search;
            return View(await query.OrderByDescending(c => c.RegisteredDate).ToListAsync());
        }

        // GET: Candidates/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var candidate = await _context.Candidates
                .Include(c => c.Applications)
                    .ThenInclude(a => a.JobPosting)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidate == null) return NotFound();
            return View(candidate);
        }

        // GET: Candidates/Create
        public IActionResult Create()
        {
            return View(new Candidate());
        }

        // POST: Candidates/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Candidate candidate, IFormFile? resumeFile)
        {
            // Remove resume file fields from validation
            ModelState.Remove("resumeFile");
            ModelState.Remove("ResumeFilePath");
            ModelState.Remove("ResumeFileName");

            if (ModelState.IsValid)
            {
                candidate.RegisteredDate = DateTime.Now;

                // Handle resume file upload
                if (resumeFile != null && resumeFile.Length > 0)
                {
                    var uploadResult = await SaveResumeFile(resumeFile);
                    if (uploadResult.Success)
                    {
                        candidate.ResumeFilePath = uploadResult.FilePath;
                        candidate.ResumeFileName = uploadResult.FileName;
                    }
                    else
                    {
                        TempData["Error"] = uploadResult.ErrorMessage;
                        return View(candidate);
                    }
                }

                _context.Add(candidate);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Candidate added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(candidate);
        }

        // GET: Candidates/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null) return NotFound();
            return View(candidate);
        }

        // POST: Candidates/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Candidate candidate, IFormFile? resumeFile, bool deleteResume = false)
        {
            if (id != candidate.Id) return NotFound();

            // Remove resume file fields from validation
            ModelState.Remove("resumeFile");
            ModelState.Remove("ResumeFilePath");
            ModelState.Remove("ResumeFileName");

            if (ModelState.IsValid)
            {
                var existingCandidate = await _context.Candidates.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                
                // Handle resume deletion
                if (deleteResume && !string.IsNullOrEmpty(existingCandidate?.ResumeFilePath))
                {
                    DeleteResumeFile(existingCandidate.ResumeFilePath);
                    candidate.ResumeFilePath = null;
                    candidate.ResumeFileName = null;
                }
                else if (resumeFile != null && resumeFile.Length > 0)
                {
                    // Delete old resume if exists
                    if (!string.IsNullOrEmpty(existingCandidate?.ResumeFilePath))
                    {
                        DeleteResumeFile(existingCandidate.ResumeFilePath);
                    }

                    // Upload new resume
                    var uploadResult = await SaveResumeFile(resumeFile);
                    if (uploadResult.Success)
                    {
                        candidate.ResumeFilePath = uploadResult.FilePath;
                        candidate.ResumeFileName = uploadResult.FileName;
                    }
                    else
                    {
                        TempData["Error"] = uploadResult.ErrorMessage;
                        return View(candidate);
                    }
                }
                else
                {
                    // Keep existing resume
                    candidate.ResumeFilePath = existingCandidate?.ResumeFilePath;
                    candidate.ResumeFileName = existingCandidate?.ResumeFileName;
                }

                _context.Update(candidate);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Candidate updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(candidate);
        }

        // POST: Candidates/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate != null)
            {
                // Delete resume file if exists
                if (!string.IsNullOrEmpty(candidate.ResumeFilePath))
                {
                    DeleteResumeFile(candidate.ResumeFilePath);
                }

                _context.Candidates.Remove(candidate);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Candidate removed.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Candidates/DownloadResume/5
        public async Task<IActionResult> DownloadResume(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null || string.IsNullOrEmpty(candidate.ResumeFilePath))
                return NotFound();

            var filePath = Path.Combine(_environment.WebRootPath, candidate.ResumeFilePath.TrimStart('/'));
            
            if (!System.IO.File.Exists(filePath))
            {
                TempData["Error"] = "Resume file not found.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, "application/octet-stream", candidate.ResumeFileName ?? "resume.pdf");
        }

        // Helper method to save resume file
        private async Task<(bool Success, string? FilePath, string? FileName, string? ErrorMessage)> SaveResumeFile(IFormFile file)
        {
            // Validate file type
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
            {
                return (false, null, null, "Only PDF, DOC, and DOCX files are allowed.");
            }

            // Validate file size (5MB limit)
            if (file.Length > 5 * 1024 * 1024)
            {
                return (false, null, null, "File size must be less than 5MB.");
            }

            try
            {
                // Create directory if it doesn't exist
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "candidates");
                Directory.CreateDirectory(uploadsFolder);

                // Generate unique filename
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return (true, $"/uploads/candidates/{uniqueFileName}", file.FileName, null);
            }
            catch (Exception ex)
            {
                return (false, null, null, $"Error uploading file: {ex.Message}");
            }
        }

        // Helper method to delete resume file
        private void DeleteResumeFile(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch
            {
                // Ignore file deletion errors
            }
        }
    }
}
