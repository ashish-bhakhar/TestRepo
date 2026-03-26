using System.ComponentModel.DataAnnotations;

namespace RecruitmentApp.Models
{
    public class JobPosting
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        [Display(Name = "Job Title")]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Employment Type")]
        public string EmploymentType { get; set; } = "Full-Time";

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Requirements { get; set; } = string.Empty;

        [Display(Name = "Salary Range")]
        public string? SalaryRange { get; set; }

        [Display(Name = "Posted Date")]
        public DateTime PostedDate { get; set; } = DateTime.Now;

        [Display(Name = "Closing Date")]
        public DateTime? ClosingDate { get; set; }

        public string Status { get; set; } = "Open";

        public ICollection<Application> Applications { get; set; } = new List<Application>();
        public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
    }

    public class Resume
    {
        public int Id { get; set; }
        public int JobPostingId { get; set; }
        public JobPosting? JobPosting { get; set; }
        
        [Required]
        public string FileName { get; set; } = string.Empty;
        
        [Required]
        public string FilePath { get; set; } = string.Empty;
        
        public long FileSize { get; set; }
        
        [Display(Name = "Uploaded Date")]
        public DateTime UploadedDate { get; set; } = DateTime.Now;
        
        public string? CandidateName { get; set; }
        public string? CandidateEmail { get; set; }
    }

    public class Candidate
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        [Display(Name = "Middle Name")]
        public string? MiddleName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? Phone { get; set; }

        [Display(Name = "Current Title")]
        public string? CurrentTitle { get; set; }

        [Display(Name = "Years of Experience")]
        public int YearsOfExperience { get; set; }

        public string? Skills { get; set; }

        [Display(Name = "LinkedIn URL")]
        public string? LinkedInUrl { get; set; }

        [Display(Name = "Resume/CV Link")]
        public string? ResumeUrl { get; set; }

        [Display(Name = "Resume File Path")]
        public string? ResumeFilePath { get; set; }

        [Display(Name = "Resume File Name")]
        public string? ResumeFileName { get; set; }

        public string? Notes { get; set; }

        [Display(Name = "Registered Date")]
        public DateTime RegisteredDate { get; set; } = DateTime.Now;

        public ICollection<Application> Applications { get; set; } = new List<Application>();

        public string FullName
        {
            get
            {
                var middle = string.IsNullOrWhiteSpace(MiddleName) ? null : MiddleName.Trim();
                return middle is null ? $"{FirstName} {LastName}" : $"{FirstName} {middle} {LastName}";
            }
        }
        
        public bool HasUploadedResume => !string.IsNullOrEmpty(ResumeFilePath);
    }

    public class Application
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Job Posting")]
        public int JobPostingId { get; set; }
        public JobPosting? JobPosting { get; set; }

        [Required]
        [Display(Name = "Candidate")]
        public int CandidateId { get; set; }
        public Candidate? Candidate { get; set; }

        [Display(Name = "Applied Date")]
        public DateTime AppliedDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Applied";

        [Display(Name = "Interview Date")]
        public DateTime? InterviewDate { get; set; }

        public string? Notes { get; set; }

        [Display(Name = "Cover Letter")]
        public string? CoverLetter { get; set; }
    }
}