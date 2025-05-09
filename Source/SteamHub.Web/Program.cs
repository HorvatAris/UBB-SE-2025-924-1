using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Context;
using SteamHub.Services;
using SteamHub.Services.Interfaces;
using SteamHub.Web.Data;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
  //  options.UseSqlServer(connectionString));
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
//    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//builder.Services.AddDbContext<DataContext>(options =>
//    options.UseSqlServer(connectionString));
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
//       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//builder.Services.AddDbContext<DataContext>(options =>
//    options.UseSqlServer(connectionString));
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();




builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<DataContext> ();


builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IGameService, GameService>();
// Register AuctionProductService
builder.Services.AddHttpClient<IGameService, GameService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:7241/api/");
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
