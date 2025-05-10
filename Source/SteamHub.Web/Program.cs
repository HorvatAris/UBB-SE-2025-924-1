using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Context;
using SteamHub.Api.Context.Repositories;
using SteamHub.ApiContract.Proxies;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.Data;
using SteamHub.Api;
using SteamHub.ApiContract.Models.User;

var builder = WebApplication.CreateBuilder(args);
var devUser = new User
{
    UserId = 5,
    Email = "liam.garcia@example.com",
    PointsBalance = 7000,
    UserName = "LiamG",
    UserRole = User.Role.User,
    WalletBalance = 55,
};

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<DataContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton(devUser);

//builder.Services.AddScoped<SteamHub.ApiContract.Models.User.User>();


builder.Services.AddScoped<IGameRepository, GameRepositoryProxy>();
builder.Services.AddScoped<IUsersGamesRepository, UserGamesRepositoryProxy>();
builder.Services.AddScoped<ITagRepository, TagRepositoryProxy>();

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IUserGameService, UserGameService>();
//ilder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartService>(provider =>
{
    var user = provider.GetRequiredService<User>();
    var userGamesRepo = provider.GetRequiredService<IUsersGamesRepository>();
    var gameRepo = provider.GetRequiredService<IGameRepository>();
    return new CartService(userGamesRepo, user, gameRepo);
});
builder.Services.AddScoped<IUserGameService>(provider =>
{
    var user = provider.GetRequiredService<User>(); // Resolve the singleton User
    var userGameRepository = provider.GetRequiredService<IUsersGamesRepository>();
    var gameRepository = provider.GetRequiredService<IGameRepository>();
    var tagRepository = provider.GetRequiredService<ITagRepository>();
    return new UserGameService(userGameRepository, gameRepository, tagRepository, user);
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
