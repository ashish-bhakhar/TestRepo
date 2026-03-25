using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Data;
using RecruitmentApp.Models;

namespace RecruitmentApp.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly RecruitmentDbContext _context;

        public ApplicationsController(RecruitmentDbContext context)
        {
            _context = context;
        }

        // GET: Applications
        public async Task<IActionResult> Index(string? status, string? search)
        {
            var query = _context.Applications
                .Include(a => a.Candidate)
                .Include(a => a.JobPosting)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == status);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(a =>
                    a.Candidate!.FirstName.Contains(search) ||
                    a.Candidate!.LastName.Contains(search) ||
                    a.JobPosting!.Title.Contains(search));

            ViewBag.CurrentStatus = status;
            ViewBag.CurrentSearch = search;
            return View(await query.OrderByDescending(a => a.AppliedDate).ToListAsync());
        }

        // GET: Applications/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var application = await _context.Applications
                .Include(a => a.Candidate)
                .Include(a => a.JobPosting)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null) return NotFound();
            return View(application);
        }

        // GET: Applications/Create
        public async Task<IActionResult> Create(int? jobId, int? candidateId)
        {
            ViewBag.Jobs = new SelectList(
                await _context.JobPostings.Where(j => j.Status == "Open").ToListAsync(),
                "Id", "Title", jobId);
            ViewBag.Candidates = new SelectList(
                await _context.Candidates.ToListAsync().ContinueWith(t =>
                    t.Result.Select(c => new { c.Id, Name = c.FullName })),
                "Id", "Name", candidateId);
            return View(new Application { JobPostingId = jobId ?? 0, CandidateId = candidateId ?? 0 });
        }

        // POST: Applications/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Application application)
        {
            // Remove navigation property validation
            ModelState.Remove("Candidate");
            ModelState.Remove("JobPosting");

            if (ModelState.IsValid)
            {
                // Check for duplicate application
                var exists = await _context.Applications.AnyAsync(a =>
                    a.CandidateId == application.CandidateId &&
                    a.JobPostingId == application.JobPostingId);

                if (exists)
                {
                    ModelState.AddModelError("", "This candidate has already applied for this position.");
                }
                else
                {
                    application.AppliedDate = DateTime.Now;
                    _context.Add(application);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Application submitted successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.Jobs = new SelectList(
                await _context.JobPostings.Where(j => j.Status == "Open").ToListAsync(),
                "Id", "Title", application.JobPostingId);
            ViewBag.Candidates = new SelectList(
                await _context.Candidates.ToListAsync().ContinueWith(t =>
                    t.Result.Select(c => new { c.Id, Name = c.FullName })),
                "Id", "Name", application.CandidateId);
            return View(application);
        }

        // GET: Applications/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null) return NotFound();

            ViewBag.Jobs = new SelectList(
                await _context.JobPostings.ToListAsync(),
                "Id", "Title", application.JobPostingId);
            ViewBag.Candidates = new SelectList(
                await _context.Candidates.ToListAsync().ContinueWith(t =>
                    t.Result.Select(c => new { c.Id, Name = c.FullName })),
                "Id", "Name", application.CandidateId);
            return View(application);
        }

        // POST: Applications/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Application application)
        {
            if (id != application.Id) return NotFound();

            ModelState.Remove("Candidate");
            ModelState.Remove("JobPosting");

            if (ModelState.IsValid)
            {
                _context.Update(application);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Application updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Jobs = new SelectList(
                await _context.JobPostings.ToListAsync(),
                "Id", "Title", application.JobPostingId);
            ViewBag.Candidates = new SelectList(
                await _context.Candidates.ToListAsync().ContinueWith(t =>
                    t.Result.Select(c => new { c.Id, Name = c.FullName })),
                "Id", "Name", application.CandidateId);
            return View(application);
        }

        // POST: Applications/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application != null)
            {
                _context.Applications.Remove(application);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Application deleted.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Applications/UpdateStatus
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application != null)
            {
                application.Status = status;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Status updated to '{status}'.";
            }
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
