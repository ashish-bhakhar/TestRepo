# TalentFlow — ASP.NET Core MVC Recruitment Management System

A complete, production-quality Recruitment Management System built with **ASP.NET Core 8 MVC**, **Entity Framework Core**, and a bespoke dark-navy/gold UI.

---

## Features

### Modules
| Module | Features |
|--------|---------|
| **Dashboard** | Live stats, recent applications, active job postings |
| **Job Postings** | Create, edit, delete, filter, view applicants per job, **multiple resume uploads** |
| **Candidates** | Profile cards, skills, contact info, application history, **resume file upload** |
| **Applications** | Full pipeline tracking, status updates, cover letters |

### Technical Highlights
- ASP.NET Core 8 MVC (Razor Views)
- **Entity Framework Core with SQL Server (LocalDB/Express/Full)**
- **Automatic database migrations and seeding**
- **Environment-based configuration (Development/Production)**
- **Multiple file upload with drag-and-drop for resumes** (PDF, DOC, DOCX)
- Full CRUD with anti-forgery token protection
- Server-side search and status filtering
- Responsive, mobile-friendly layout
- Custom dark-navy + gold design system (Syne + Outfit fonts)

---

## Project Structure

```
RecruitmentApp/
├── Controllers/
│   ├── HomeController.cs          # Dashboard stats
│   ├── JobPostingsController.cs   # Job CRUD
│   ├── CandidatesController.cs    # Candidate CRUD
│   └── ApplicationsController.cs  # Application CRUD + status pipeline
├── Models/
│   └── Models.cs                  # JobPosting, Candidate, Application
├── Data/
│   └── RecruitmentDbContext.cs    # EF Context + seed data
├── Views/
│   ├── Shared/_Layout.cshtml      # Sidebar navigation layout
│   ├── Home/Index.cshtml          # Dashboard
│   ├── JobPostings/               # List, Create, Edit, Details
│   ├── Candidates/                # List (cards), Create, Edit, Details
│   └── Applications/              # List, Create, Edit, Details
├── wwwroot/
│   ├── css/site.css               # Full design system
│   └── js/site.js                 # Search auto-submit, alerts
├── Program.cs                     # App bootstrap
└── RecruitmentApp.csproj
```

---

## Quick Start

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download) — check: `dotnet --version`
- **Optional:** SQL Server LocalDB/Express (for persistent database)

### Run in 3 steps

```bash
# 1. Enter project folder
cd RecruitmentApp

# 2. Restore packages
dotnet restore

# 3. Start the app
dotnet run
```

Open your browser to: **http://localhost:5000**

**By default**, the app uses **In-Memory database** in Development mode (no SQL Server needed). Data will reset when you restart the app.

---

## Database Options

### Option 1: In-Memory Database (Default - No Setup)
Perfect for quick testing and development.

**Already configured!** Just run `dotnet run`

### Option 2: SQL Server Database (Production-Ready)
For persistent data storage.

**Quick Setup with LocalDB (Windows):**

1. Install SQL Server LocalDB (comes with Visual Studio)
2. Edit `appsettings.json` and set: `"UseInMemoryDatabase": false`
3. Run: `dotnet run`

The database will be created automatically!

**📖 Full SQL Server Setup Guide:** See [SQL_SERVER_SETUP.md](SQL_SERVER_SETUP.md) for detailed instructions including:
- LocalDB setup (Windows)
- SQL Express setup (Windows/Linux/Mac)
- Full SQL Server / Azure SQL setup
- Connection strings for all scenarios
- Migration management
- Troubleshooting

---

## Switching to SQL Server

By default the app uses **In-Memory database** in Development and **SQL Server** in Production.

**To use SQL Server in Development:**

Edit `appsettings.Development.json`:
```json
{
  "UseInMemoryDatabase": false
}
```

Or set in `appsettings.json`:
```json
{
  "UseInMemoryDatabase": false,
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RecruitmentDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

---

## Application Status Pipeline

Applications move through this pipeline:

```
Applied → Under Review → Interview → Offered → Hired
                                              ↘ Rejected
```

Click any stage in the status bar on the Application Details page to update instantly.

---

## Resume Upload Feature

On any Job Posting details page, you can:

- **Upload multiple resumes at once** (drag-and-drop or click to browse)
- Supported formats: PDF, DOC, DOCX (max 10MB per file)
- Add optional candidate name and email with each upload
- Download any resume with one click
- Delete resumes individually
- All files are stored in `/wwwroot/uploads/resumes/`

**Note:** When using In-Memory database, uploaded files persist in the file system even when the app restarts, but the database records will be lost. Switch to SQL Server for production use.

---

## Candidate Resume Upload Feature

When adding or editing candidates, you can:

- **Upload resume files** directly (drag-and-drop or click to browse)
- Supported formats: PDF, DOC, DOCX (max 5MB)
- **OR** provide an external link to resume/CV
- Download uploaded resumes with one click
- Replace or delete existing resume files
- All files stored in `/wwwroot/uploads/candidates/`

This works alongside the job posting resume upload feature, giving you two ways to manage resumes:
1. **Job Posting Resumes**: Multiple anonymous resumes per job
2. **Candidate Resumes**: One resume per candidate profile

---

## Sample Data

Seeded automatically on startup:

**Job Postings:** Senior Software Engineer, Product Manager, UX Designer, Marketing Specialist

**Candidates:** Alex Johnson, Maria Garcia, James Chen, Sarah Williams

**Applications:** 4 applications linking candidates to jobs with different statuses

---

## Design System

| Token | Value |
|-------|-------|
| Background | `#0d1b2e` (deep navy) |
| Accent | `#c9a84c` (gold) |
| Display Font | Syne (700/800) |
| Body Font | Outfit (300–600) |
| Border radius | 8–14px |
| Cards | Semi-transparent with border |

---

## Extending the App

### Add Authentication
```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
```

### Add File Upload (Resumes)
Add `IFormFile` to the Candidate model and configure `wwwroot/uploads/`.

### Add Email Notifications
Use `MailKit` to send emails on status changes.

### Add Reporting
Use `ClosedXML` or `QuestPDF` for Excel/PDF reports.

---

## License
MIT — free for personal and commercial use.
