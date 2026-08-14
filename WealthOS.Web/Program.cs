using Microsoft.EntityFrameworkCore;
using WealthOS.Application;
using WealthOS.Infrastructure;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// DDD layering wired up here: Domain has no dependencies, Application depends on Domain,
// Infrastructure implements Domain's interfaces and depends on Application only for DI wiring order,
// Web depends on Application's abstractions (IFinanceService) and never touches Infrastructure directly
// except for this composition-root registration.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ICurrentUserContext, CurrentUserContext>();

var app = builder.Build();

// Ensure the SQLite database + schema exist on startup (MVP convenience; use real EF Core
// migrations via `dotnet ef migrations add` + `dotnet ef database update` for production).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WealthOsDbContext>();
    db.Database.EnsureCreated();
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
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
