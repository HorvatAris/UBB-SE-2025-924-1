using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Proxies;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web;
using SteamHub.Web.Services;

var builder = WebApplication.CreateBuilder(args);
//var devUser = new User
//{
//    UserId = 5,
//    Email = "liam.garcia@example.com",
//    PointsBalance = 7000,
//    UserName = "LiamG",
//    UserRole = User.Role.User,
//    WalletBalance = 55,
//};



builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
//builder.Services.AddSingleton(devUser);

//builder.Services.AddScoped<SteamHub.ApiContract.Models.User.User>();



builder.Services.AddHttpContextAccessor();


builder.Services.AddScoped<IUserDetails, WebUserDetails>();


builder.Services.AddScoped<IGameRepository, GameRepositoryProxy>();
builder.Services.AddScoped<IUsersGamesRepository, UserGamesRepositoryProxy>();
builder.Services.AddScoped<ITagRepository, TagRepositoryProxy>();

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IUserGameService, UserGameService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartService,CartService>();
builder.Services.AddScoped<IAuthManager, AuthManager>();
//ilder.Services.AddScoped<ICartService, CartService>();
//builder.Services.AddScoped<ICartService>(provider =>
//{
//    var user = provider.GetRequiredService<User>();
//    var userGamesRepo = provider.GetRequiredService<IUsersGamesRepository>();
//    var gameRepo = provider.GetRequiredService<IGameRepository>();
//    return new CartService(userGamesRepo, user, gameRepo);
//});
//builder.Services.AddScoped<IUserGameService>(provider =>
//{
//    var user = provider.GetRequiredService<User>(); // Resolve the singleton User
//    var userGameRepository = provider.GetRequiredService<IUsersGamesRepository>();
//    var gameRepository = provider.GetRequiredService<IGameRepository>();
//    var tagRepository = provider.GetRequiredService<ITagRepository>();
//    return new UserGameService(userGameRepository, gameRepository, tagRepository, user);
//});


var apiBaseUri = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);

builder.Services.AddHttpClient("AuthApi", client =>
{
    client.BaseAddress = apiBaseUri;
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    
}).ConfigurePrimaryHttpMessageHandler(() => new NoSslCertificateValidationHandler());


builder.Services.AddHttpClient("SteamHubApi", client =>
{
    client.BaseAddress = apiBaseUri;
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    
}).ConfigurePrimaryHttpMessageHandler(() => new NoSslCertificateValidationHandler());


builder.Services.AddAuthentication("SteamHubAuth")
    .AddCookie("SteamHubAuth", options =>
    {
        options.LoginPath = "/Authentication/Login";
        options.AccessDeniedPath = "/Authentication/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    });

builder.Services.AddAuthorization();

builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10); // Set timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
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

app.UseAuthentication();
app.UseAuthorization(); // Always place this after UseAuthentication
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// app.MapRazorPages();

app.Run();
