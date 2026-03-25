using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add MVC
builder.Services.AddControllersWithViews();

// Add EF Core - Choose database based on environment or configuration
var useInMemoryDb = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");

if (useInMemoryDb)
{
    // In-Memory Database (for development/testing)
    builder.Services.AddDbContext<RecruitmentDbContext>(options =>
        options.UseInMemoryDatabase("RecruitmentDb"));
}
else
{
    // SQL Server Database (for production)
    builder.Services.AddDbContext<RecruitmentDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

var app = builder.Build();

// Initialize and seed the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RecruitmentDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        if (useInMemoryDb)
        {
            // For In-Memory, ensure database is created
            context.Database.EnsureCreated();
            logger.LogInformation("In-Memory database created successfully");
        }
        else
        {
            // For SQL Server, apply pending migrations
            logger.LogInformation("Applying database migrations...");
            context.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully");
        }
        
        // Seed the database with sample data
        DbSeeder.SeedData(context);
        logger.LogInformation("Database seeded successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
