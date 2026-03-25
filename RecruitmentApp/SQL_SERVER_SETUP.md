# SQL Server Database Setup Guide

This guide shows you how to set up and use SQL Server with the TalentFlow Recruitment App.

---

## Database Configuration Options

The app supports **two database modes**:

### 1. **In-Memory Database** (Default for Development)
- No setup required
- Data resets when app restarts
- Perfect for testing and development
- Enabled by default in Development environment

### 2. **SQL Server Database** (Production)
- Persistent data storage
- Production-ready
- Requires SQL Server installation
- Can use LocalDB, SQL Express, or full SQL Server

---

## Quick Start with In-Memory Database

No setup needed! Just run:

```bash
dotnet run
```

The app will automatically use in-memory database in Development mode.

---

## Setting Up SQL Server Database

### Option 1: Using SQL Server LocalDB (Easiest - Windows Only)

LocalDB comes with Visual Studio and is perfect for development.

**1. Install SQL Server LocalDB**
- Download from: https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb
- Or install Visual Studio which includes it

**2. Verify Installation**
```bash
sqllocaldb info
```

**3. Update appsettings.json**
```json
{
  "UseInMemoryDatabase": false,
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RecruitmentDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

**4. Run the App**
```bash
dotnet run
```

The database will be created automatically with migrations!

---

### Option 2: Using SQL Server Express (Free)

**1. Download and Install SQL Server Express**
- Download from: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
- Choose "Express" edition (free)
- Install with default settings

**2. Install SQL Server Management Studio (SSMS) - Optional**
- Download from: https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms
- Use this to view and manage your database visually

**3. Update Connection String**

Edit `appsettings.json`:

```json
{
  "UseInMemoryDatabase": false,
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=RecruitmentDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

**4. Run the App**
```bash
dotnet run
```

---

### Option 3: Using Full SQL Server or Azure SQL

**For SQL Server with Authentication:**

```json
{
  "UseInMemoryDatabase": false,
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=RecruitmentDb;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

**For Azure SQL Database:**

```json
{
  "UseInMemoryDatabase": false,
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:YOUR_SERVER.database.windows.net,1433;Database=RecruitmentDb;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=true"
  }
}
```

---

## Environment-Specific Configuration

The app uses different settings based on environment:

### Development Environment
- Uses `appsettings.Development.json`
- Default: In-Memory database
- Sample data automatically seeded

### Production Environment
- Uses `appsettings.Production.json`
- Default: SQL Server database
- Update connection string before deploying

### Switching Environments

**Windows:**
```bash
$env:ASPNETCORE_ENVIRONMENT="Production"
dotnet run
```

**Mac/Linux:**
```bash
export ASPNETCORE_ENVIRONMENT=Production
dotnet run
```

---

## Database Migrations

The app uses **Entity Framework Core Migrations** to manage database schema.

### Automatic Migration (Recommended)

The app automatically applies migrations on startup when using SQL Server. Just run:

```bash
dotnet run
```

### Manual Migration Commands

If you need to manage migrations manually:

**View current migrations:**
```bash
dotnet ef migrations list
```

**Create a new migration (after model changes):**
```bash
dotnet ef migrations add YourMigrationName
```

**Apply migrations manually:**
```bash
dotnet ef database update
```

**Rollback to previous migration:**
```bash
dotnet ef database update PreviousMigrationName
```

**Remove last migration (if not applied):**
```bash
dotnet ef migrations remove
```

**Generate SQL script:**
```bash
dotnet ef migrations script -o migration.sql
```

---

## Database Seeding

The app automatically seeds sample data on first run:
- 4 Job Postings
- 4 Candidates
- 4 Applications

**To reseed the database:**

1. Delete the database
2. Restart the app

**For LocalDB:**
```bash
dotnet ef database drop
dotnet run
```

**Or manually in SSMS:**
```sql
DROP DATABASE RecruitmentDb;
```

---

## Troubleshooting

### Issue: "Cannot open database"

**Solution:** Make sure SQL Server service is running

**Windows:**
```bash
# Check service status
sc query MSSQL$SQLEXPRESS

# Start service
net start MSSQL$SQLEXPRESS
```

### Issue: "Login failed for user"

**Solution:** Check connection string credentials or use Windows Authentication (Trusted_Connection=True)

### Issue: "Server not found"

**Solution:** Verify server name

**Find LocalDB instances:**
```bash
sqllocaldb info
sqllocaldb info mssqllocaldb
```

**Find SQL Server instances:**
```bash
sqlcmd -L
```

### Issue: Migrations not applying

**Solution:** Ensure you have the EF Core tools installed

```bash
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```

---

## Connection String Reference

| Component | Description | Example |
|-----------|-------------|---------|
| `Server` | SQL Server instance | `localhost`, `(localdb)\mssqllocaldb`, `tcp:server.database.windows.net,1433` |
| `Database` | Database name | `RecruitmentDb` |
| `Trusted_Connection` | Use Windows Auth | `True` or `False` |
| `User Id` | SQL Auth username | `sa`, `recruitment_user` |
| `Password` | SQL Auth password | `YourPassword123!` |
| `TrustServerCertificate` | Skip cert validation | `True` (dev only) |
| `MultipleActiveResultSets` | Enable MARS | `True` |

---

## Production Deployment Checklist

- [ ] Update connection string in `appsettings.Production.json`
- [ ] Set `UseInMemoryDatabase` to `false`
- [ ] Create database on production SQL Server
- [ ] Ensure firewall allows database connections
- [ ] Test connection string locally first
- [ ] Set strong password for SQL authentication
- [ ] Enable SSL/TLS for database connections
- [ ] Configure backup schedule for database
- [ ] Set appropriate user permissions
- [ ] Remove or protect development seed data

---

## Viewing Your Database

### Using SQL Server Management Studio (SSMS)

1. Open SSMS
2. Connect to your server instance
3. Expand Databases → RecruitmentDb
4. Browse tables: JobPostings, Candidates, Applications, Resumes

### Using Azure Data Studio (Cross-Platform)

1. Download from: https://learn.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio
2. Connect to your database
3. View tables and data

### Using dotnet ef CLI

```bash
# View database info
dotnet ef dbcontext info

# View model configuration
dotnet ef dbcontext scaffold "ConnectionString" Microsoft.EntityFrameworkCore.SqlServer
```

---

## Need Help?

**EF Core Documentation:** https://learn.microsoft.com/en-us/ef/core/  
**SQL Server Documentation:** https://learn.microsoft.com/en-us/sql/  
**Connection Strings Reference:** https://www.connectionstrings.com/sql-server/
