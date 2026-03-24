using RecruitmentApp.Models;

namespace RecruitmentApp.Data
{
    public static class DbSeeder
    {
        public static void SeedData(RecruitmentDbContext context)
        {
            // Check if data already exists
            if (context.JobPostings.Any())
            {
                return; // Database has been seeded
            }

            // Seed Job Postings
            var jobs = new[]
            {
                new JobPosting
                {
                    Title = "Senior Software Engineer", Department = "Engineering",
                    Location = "New York, NY", EmploymentType = "Full-Time",
                    Description = "We are looking for a Senior Software Engineer to join our growing engineering team. You will design, develop, and maintain scalable software solutions.",
                    Requirements = "5+ years experience in C# or Java\nStrong knowledge of cloud platforms (AWS/Azure)\nExperience with microservices architecture\nExcellent problem-solving skills",
                    SalaryRange = "$120,000 - $160,000", Status = "Open",
                    PostedDate = DateTime.Now.AddDays(-10), ClosingDate = DateTime.Now.AddDays(20)
                },
                new JobPosting
                {
                    Title = "Product Manager", Department = "Product",
                    Location = "San Francisco, CA", EmploymentType = "Full-Time",
                    Description = "Lead product strategy and roadmap for our flagship product. Work closely with engineering, design, and business stakeholders.",
                    Requirements = "3+ years of product management experience\nStrong analytical and communication skills\nExperience with Agile methodologies\nTechnical background preferred",
                    SalaryRange = "$110,000 - $140,000", Status = "Open",
                    PostedDate = DateTime.Now.AddDays(-5), ClosingDate = DateTime.Now.AddDays(25)
                },
                new JobPosting
                {
                    Title = "UX Designer", Department = "Design",
                    Location = "Remote", EmploymentType = "Full-Time",
                    Description = "Create intuitive and beautiful user experiences for our web and mobile products.",
                    Requirements = "Portfolio demonstrating UX/UI work\nProficiency in Figma or Sketch\nExperience with user research and testing\n3+ years of UX design experience",
                    SalaryRange = "$90,000 - $120,000", Status = "Open",
                    PostedDate = DateTime.Now.AddDays(-3), ClosingDate = DateTime.Now.AddDays(30)
                },
                new JobPosting
                {
                    Title = "Marketing Specialist", Department = "Marketing",
                    Location = "Chicago, IL", EmploymentType = "Full-Time",
                    Description = "Drive marketing campaigns and brand awareness initiatives across digital and traditional channels.",
                    Requirements = "2+ years of marketing experience\nExperience with digital marketing tools\nStrong writing and communication skills\nData-driven mindset",
                    SalaryRange = "$65,000 - $85,000", Status = "Closed",
                    PostedDate = DateTime.Now.AddDays(-30), ClosingDate = DateTime.Now.AddDays(-5)
                }
            };

            context.JobPostings.AddRange(jobs);
            context.SaveChanges();

            // Seed Candidates
            var candidates = new[]
            {
                new Candidate
                {
                    FirstName = "Alex", LastName = "Johnson", Email = "alex.johnson@email.com",
                    Phone = "+1-555-0101", CurrentTitle = "Software Engineer", YearsOfExperience = 6,
                    Skills = "C#, .NET, Azure, Docker, Kubernetes", LinkedInUrl = "linkedin.com/in/alexjohnson",
                    Notes = "Strong technical background, excellent communicator", RegisteredDate = DateTime.Now.AddDays(-8)
                },
                new Candidate
                {
                    FirstName = "Maria", LastName = "Garcia", Email = "maria.garcia@email.com",
                    Phone = "+1-555-0102", CurrentTitle = "Senior Product Manager", YearsOfExperience = 5,
                    Skills = "Product Strategy, Agile, Jira, Data Analysis, Roadmapping", LinkedInUrl = "linkedin.com/in/mariagarcia",
                    Notes = "Previously at a Fortune 500 company", RegisteredDate = DateTime.Now.AddDays(-4)
                },
                new Candidate
                {
                    FirstName = "James", LastName = "Chen", Email = "james.chen@email.com",
                    Phone = "+1-555-0103", CurrentTitle = "UX Lead Designer", YearsOfExperience = 7,
                    Skills = "Figma, Sketch, User Research, Prototyping, Design Systems", LinkedInUrl = "linkedin.com/in/jameschen",
                    Notes = "Award-winning design portfolio", RegisteredDate = DateTime.Now.AddDays(-2)
                },
                new Candidate
                {
                    FirstName = "Sarah", LastName = "Williams", Email = "sarah.williams@email.com",
                    Phone = "+1-555-0104", CurrentTitle = "Full Stack Developer", YearsOfExperience = 4,
                    Skills = "React, Node.js, TypeScript, PostgreSQL, AWS", LinkedInUrl = "linkedin.com/in/sarahwilliams",
                    Notes = "Open source contributor", RegisteredDate = DateTime.Now.AddDays(-6)
                }
            };

            context.Candidates.AddRange(candidates);
            context.SaveChanges();

            // Seed Applications
            var applications = new[]
            {
                new Application
                {
                    JobPostingId = jobs[0].Id, CandidateId = candidates[0].Id, Status = "Interview",
                    AppliedDate = DateTime.Now.AddDays(-7), InterviewDate = DateTime.Now.AddDays(3),
                    CoverLetter = "I am excited to apply for the Senior Software Engineer position...",
                    Notes = "Strong candidate, passed technical screening"
                },
                new Application
                {
                    JobPostingId = jobs[1].Id, CandidateId = candidates[1].Id, Status = "Under Review",
                    AppliedDate = DateTime.Now.AddDays(-3),
                    CoverLetter = "With 5 years of product management experience...",
                    Notes = "Impressive background"
                },
                new Application
                {
                    JobPostingId = jobs[2].Id, CandidateId = candidates[2].Id, Status = "Applied",
                    AppliedDate = DateTime.Now.AddDays(-1),
                    CoverLetter = "As a UX Lead with 7 years of experience..."
                },
                new Application
                {
                    JobPostingId = jobs[0].Id, CandidateId = candidates[3].Id, Status = "Applied",
                    AppliedDate = DateTime.Now.AddDays(-5),
                    CoverLetter = "I believe my full-stack skills would be a great fit..."
                }
            };

            context.Applications.AddRange(applications);
            context.SaveChanges();
        }
    }
}
