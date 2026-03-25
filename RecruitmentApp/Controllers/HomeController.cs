using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Data;

namespace RecruitmentApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly RecruitmentDbContext _context;

        public HomeController(RecruitmentDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalJobs = await _context.JobPostings.CountAsync();
            ViewBag.OpenJobs = await _context.JobPostings.CountAsync(j => j.Status == "Open");
            ViewBag.TotalCandidates = await _context.Candidates.CountAsync();
            ViewBag.TotalApplications = await _context.Applications.CountAsync();
            ViewBag.InterviewsScheduled = await _context.Applications.CountAsync(a => a.Status == "Interview");
            ViewBag.Hired = await _context.Applications.CountAsync(a => a.Status == "Hired");

            var recentApplications = await _context.Applications
                .Include(a => a.Candidate)
                .Include(a => a.JobPosting)
                .OrderByDescending(a => a.AppliedDate)
                .Take(5)
                .ToListAsync();

            var recentJobs = await _context.JobPostings
                .OrderByDescending(j => j.PostedDate)
                .Take(4)
                .ToListAsync();

            ViewBag.RecentApplications = recentApplications;
            ViewBag.RecentJobs = recentJobs;

            return View();
        }
    }
}
